#!/usr/bin/env python3
from __future__ import annotations

import argparse
import ast
import json
import re
import shlex
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Iterable
from xml.etree import ElementTree


DEFAULT_INCLUDE_EXTENSIONS = {".xaml", ".cs", ".json", ".config", ".md"}
DEFAULT_EXCLUDE_DIRS = {
    ".git",
    ".vs",
    "bin",
    "obj",
    ".dotnet",
}

# Keep common ASCII and CJK punctuation even if the scanner misses some runtime-only text.
DEFAULT_BASE_CHARACTERS = (
    " \t\r\n"
    "0123456789"
    "abcdefghijklmnopqrstuvwxyz"
    "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    r"""!"#$%&'()*+,-./:;<=>?@[\]^_`{|}~"""
    "，。！？；：、“”‘’（）【】《》〈〉「」『』〔〕"
    "…—-·•～￥"
)


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Generate a glyph subset text file from repo content and optionally run pyftsubset."
    )
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parents[1],
        help="Repository root to scan. Defaults to the repo containing this script.",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=Path("Fonts/subset.txt"),
        help="Subset text output path, relative to --root unless absolute.",
    )
    parser.add_argument(
        "--font",
        action="append",
        default=[],
        help="Font file to subset. Can be specified multiple times. Relative to --root unless absolute.",
    )
    parser.add_argument(
        "--subset-output-dir",
        type=Path,
        default=Path("Fonts/subset"),
        help="Directory for generated subset font files when --run-pyftsubset is used.",
    )
    parser.add_argument(
        "--pyftsubset",
        default="py -m fontTools.subset",
        help="Command used to invoke fontTools subsetter. Examples: 'py -m fontTools.subset' or 'pyftsubset'.",
    )
    parser.add_argument(
        "--run-pyftsubset",
        action="store_true",
        help="Run pyftsubset after generating the subset text file.",
    )
    parser.add_argument(
        "--no-default-base",
        action="store_true",
        help="Do not pre-seed the subset with built-in ASCII and punctuation.",
    )
    parser.add_argument(
        "--extra-text-file",
        action="append",
        default=[],
        help="Additional text file whose raw content should be included in the subset. Can be specified multiple times.",
    )
    parser.add_argument(
        "--include-ext",
        action="append",
        default=[],
        help="Extra file extension to scan, for example .txt. Can be specified multiple times.",
    )
    parser.add_argument(
        "--exclude-dir",
        action="append",
        default=[],
        help="Extra directory name to exclude from scanning. Can be specified multiple times.",
    )
    parser.add_argument(
        "--verbose",
        action="store_true",
        help="Print scanned files and generated pyftsubset commands.",
    )
    return parser.parse_args()


def resolve_path(root: Path, value: Path) -> Path:
    return value if value.is_absolute() else root / value


def iter_scan_files(root: Path, include_extensions: set[str], exclude_dirs: set[str]) -> Iterable[Path]:
    for path in root.rglob("*"):
        if not path.is_file():
            continue
        if any(part in exclude_dirs for part in path.parts):
            continue
        if path.suffix.lower() in include_extensions:
            yield path


def collect_from_xaml(path: Path) -> set[str]:
    chars: set[str] = set()
    try:
        root = ElementTree.parse(path).getroot()
    except ElementTree.ParseError:
        return chars

    for element in root.iter():
        if element.text:
            chars.update(element.text)
        if element.tail:
            chars.update(element.tail)
        for value in element.attrib.values():
            # XAML attributes include a lot of markup; keep only user-facing pieces.
            if any(token in value for token in ("{Binding", "{StaticResource", "{DynamicResource", "pack://", "/Fonts/#")):
                continue
            chars.update(value)
    return chars


def collect_from_cs(path: Path) -> set[str]:
    chars: set[str] = set()
    text = path.read_text(encoding="utf-8")

    token_pattern = re.compile(r'@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"')

    for match in token_pattern.finditer(text):
        literal = match.group(0)
        if literal.startswith('@"'):
            chars.update(literal[2:-1].replace('""', '"'))
            continue

        try:
            value = ast.literal_eval(literal)
        except Exception:
            continue
        chars.update(value)

    return chars


def collect_from_json(path: Path) -> set[str]:
    chars: set[str] = set()
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError:
        chars.update(path.read_text(encoding="utf-8"))
        return chars

    def walk(value: object) -> None:
        if isinstance(value, str):
            chars.update(value)
        elif isinstance(value, list):
            for item in value:
                walk(item)
        elif isinstance(value, dict):
            for key, item in value.items():
                chars.update(str(key))
                walk(item)

    walk(data)
    return chars


def collect_generic_text(path: Path) -> set[str]:
    return set(path.read_text(encoding="utf-8"))


def build_subset_characters(args: argparse.Namespace) -> tuple[list[Path], list[str]]:
    root = args.root.resolve()
    include_extensions = DEFAULT_INCLUDE_EXTENSIONS | {ext if ext.startswith(".") else f".{ext}" for ext in args.include_ext}
    exclude_dirs = DEFAULT_EXCLUDE_DIRS | set(args.exclude_dir)

    characters: set[str] = set()
    if not args.no_default_base:
        characters.update(DEFAULT_BASE_CHARACTERS)

    scanned_files: list[Path] = []
    for path in iter_scan_files(root, include_extensions, exclude_dirs):
        scanned_files.append(path)
        suffix = path.suffix.lower()
        try:
            if suffix == ".xaml":
                characters.update(collect_from_xaml(path))
            elif suffix == ".cs":
                characters.update(collect_from_cs(path))
            elif suffix == ".json":
                characters.update(collect_from_json(path))
            else:
                characters.update(collect_generic_text(path))
        except UnicodeDecodeError:
            continue

    for extra_file in args.extra_text_file:
        extra_path = resolve_path(root, Path(extra_file))
        if extra_path.exists():
            characters.update(extra_path.read_text(encoding="utf-8"))

    # Drop NUL and normalize ordering for stable diffs.
    characters.discard("\x00")
    ordered = sorted(characters)
    return scanned_files, ordered


def write_subset_file(output_path: Path, characters: list[str]) -> None:
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text("".join(characters), encoding="utf-8")


def run_pyftsubset(args: argparse.Namespace, root: Path, subset_text_path: Path) -> list[Path]:
    output_dir = resolve_path(root, args.subset_output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    generated: list[Path] = []
    pyftsubset_command = shlex.split(args.pyftsubset, posix=False)
    if not pyftsubset_command:
        raise ValueError("--pyftsubset cannot be empty")

    executable = pyftsubset_command[0]
    if shutil.which(executable) is None and not Path(executable).exists():
        raise FileNotFoundError(
            f"Unable to find pyftsubset command '{args.pyftsubset}'. "
            "Install fonttools or pass --pyftsubset explicitly."
        )

    for font_value in args.font:
        font_path = resolve_path(root, Path(font_value))
        if not font_path.exists():
            raise FileNotFoundError(f"Font file not found: {font_path}")

        output_path = output_dir / f"{font_path.stem}.subset{font_path.suffix}"
        command = [
            *pyftsubset_command,
            str(font_path),
            f"--text-file={subset_text_path}",
            f"--output-file={output_path}",
            "--layout-features=*",
            "--glyph-names",
            "--symbol-cmap",
            "--legacy-cmap",
            "--notdef-glyph",
            "--notdef-outline",
            "--recommended-glyphs",
            "--hinting",
        ]

        if args.verbose:
            print("Running:", " ".join(command))

        subprocess.run(command, check=True)
        generated.append(output_path)

    return generated


def main() -> int:
    args = parse_args()
    root = args.root.resolve()
    subset_output = resolve_path(root, args.output)

    scanned_files, characters = build_subset_characters(args)
    write_subset_file(subset_output, characters)

    print(f"Scanned {len(scanned_files)} files")
    print(f"Generated subset text: {subset_output}")
    print(f"Unique characters: {len(characters)}")

    if args.verbose:
        for path in scanned_files:
            print(f"  {path.relative_to(root)}")

    if args.run_pyftsubset:
        if not args.font:
            print("--run-pyftsubset requires at least one --font", file=sys.stderr)
            return 2
        generated_fonts = run_pyftsubset(args, root, subset_output)
        print("Generated subset fonts:")
        for path in generated_fonts:
            print(f"  {path}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
