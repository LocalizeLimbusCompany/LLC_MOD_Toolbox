using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox.Services.Skin
{
    public interface ISkinMusicService
    {
        void PlayWithFadeIn(string musicPath);
        void FadeOutAndStop();
        Task FadeOutAndStopAsync();
        void SetVolumeMultiplier(double multiplier);
    }

    public sealed class SkinMusicService : ISkinMusicService
    {
        private static readonly TimeSpan FadeInDuration = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan FadeOutDuration = TimeSpan.FromSeconds(0.5);
        private const double TargetVolume = 0.35;

        private readonly Dispatcher _dispatcher;
        private MediaPlayer? _player;
        private DispatcherTimer? _fadeTimer;
        private string? _currentMusicPath;
        private double _volumeMultiplier = 1;

        public SkinMusicService()
        {
            _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public void PlayWithFadeIn(string musicPath)
        {
            if (string.IsNullOrWhiteSpace(musicPath) || !File.Exists(musicPath))
                return;

            _dispatcher.Invoke(() =>
            {
                bool sameMusic = string.Equals(_currentMusicPath, musicPath, StringComparison.OrdinalIgnoreCase);
                if (_player == null || !sameMusic)
                {
                    ClosePlayer();
                    _player = new MediaPlayer();
                    _player.MediaEnded += OnMediaEnded;
                    _player.Volume = 0;
                    _player.Open(new Uri(musicPath, UriKind.Absolute));
                    _currentMusicPath = musicPath;
                }

                _player.Play();
                FadeTo(GetTargetVolume(), FadeInDuration);
            });
        }

        public void FadeOutAndStop()
        {
            _ = FadeOutAndStopAsync();
        }

        public Task FadeOutAndStopAsync()
        {
            var completion = new TaskCompletionSource();

            _dispatcher.Invoke(() =>
            {
                if (_player == null)
                {
                    completion.SetResult();
                    return;
                }

                FadeTo(0, FadeOutDuration, () =>
                {
                    ClosePlayer();
                    completion.SetResult();
                });
            });

            return completion.Task;
        }

        public void SetVolumeMultiplier(double multiplier)
        {
            _dispatcher.Invoke(() =>
            {
                _volumeMultiplier = Math.Clamp(multiplier, 0, 1);
                if (_player != null)
                    FadeTo(GetTargetVolume(), TimeSpan.FromMilliseconds(500));
            });
        }

        private void FadeTo(double targetVolume, TimeSpan duration, Action? completed = null)
        {
            _fadeTimer?.Stop();
            _fadeTimer = null;

            if (_player == null)
            {
                completed?.Invoke();
                return;
            }

            double startVolume = _player.Volume;
            var stopwatch = Stopwatch.StartNew();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            timer.Tick += (_, _) =>
            {
                if (_player == null)
                {
                    timer.Stop();
                    completed?.Invoke();
                    return;
                }

                double progress = duration.TotalMilliseconds <= 0
                    ? 1
                    : Math.Min(1, stopwatch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds);
                _player.Volume = startVolume + (targetVolume - startVolume) * progress;

                if (progress < 1)
                    return;

                timer.Stop();
                _fadeTimer = null;
                completed?.Invoke();
            };

            _fadeTimer = timer;
            timer.Start();
        }

        private double GetTargetVolume()
        {
            return TargetVolume * _volumeMultiplier;
        }

        private void OnMediaEnded(object? sender, EventArgs e)
        {
            if (_player == null)
                return;

            _player.Position = TimeSpan.Zero;
            _player.Play();
        }

        private void ClosePlayer()
        {
            _fadeTimer?.Stop();
            _fadeTimer = null;

            if (_player != null)
            {
                _player.MediaEnded -= OnMediaEnded;
                _player.Stop();
                _player.Close();
                _player = null;
            }

            _currentMusicPath = null;
        }
    }
}
