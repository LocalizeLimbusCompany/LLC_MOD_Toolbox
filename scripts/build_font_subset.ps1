#!/usr/bin/env pwsh
# 生成三个字体的 subset 文件

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir

uv run "$scriptDir/font_subset.py" `
    --root "$repoRoot" `
    --font "Fonts/SourceHanSansCN-Bold.ttf" `
    --font "Fonts/SourceHanSansCN-Heavy.ttf" `
    --font "Fonts/SourceHanSansCN-Medium.ttf" `
    --run-pyftsubset
