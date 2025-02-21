using System;
using UniRx;
using UnityEngine;

namespace UBW.SequenceAnimation
{
    public enum OnEnableBehavior
    {
        None,
        Play,
        Resume,
    }

    public enum OnDisableBehavior
    {
        None,
        Pause,
        Stop,
    }

    public abstract class SequenceAnimation<T> : MonoBehaviour where T : Component
    {
        protected abstract T component { get; }

        [SerializeField] private Sprite[] sprites;
        /// <summary> Sets animation sequence. </summary>
        /// <remarks> When called, it would cancel current sequence animation. </remarks>
        public void SetAnimationSequence(Sprite[] sprites)
        {
            if (!ValidateSprites(sprites))
            {
                Debug.LogError("Sprites cannot be null or empty.");
                return;
            }

            this.sprites = sprites;

            serialDisposable.Disposable?.Dispose();
            serialDisposable.Disposable = null;

            ResetSequenceAnimation();
        }

        private bool ValidateSprites(Sprite[] sprites) => sprites != null && sprites.Length > 0;

        [SerializeField] private OnEnableBehavior onEnableBehavior;
        /// <summary> Gets or sets the behavior to execute when the GameObject is enabled. </summary>
        public OnEnableBehavior OnEnableBehavior
        {
            get => onEnableBehavior;
            set => onEnableBehavior = value;
        }
        [SerializeField] private OnDisableBehavior onDisableBehavior;
        /// <summary> Gets or sets the behavior to execute when the GameObject is disabled. </summary>
        public OnDisableBehavior OnDisableBehavior
        {
            get => onDisableBehavior;
            set => onDisableBehavior = value;
        }

        [Header("Parameters")]
        [Min(0f)][SerializeField] private float fps;
        public float Fps
        {
            get => fps;
            set => fps = Mathf.Max(0f, value);
        }

        [Tooltip("Set to -1 for infinite loops")]
        [Min(-1)][SerializeField] private int loops;
        /// <remarks> Sets to -1 for infinite loops. </remarks>
        public int Loops
        {
            get => loops;
            set => loops = Mathf.Max(-1, value);
        }

        private SerialDisposable serialDisposable = new SerialDisposable();

        private Subject<Unit> onPlayedSubject = new Subject<Unit>();
        private Subject<Unit> onPausedSubject = new Subject<Unit>();
        private Subject<Unit> onResumedSubject = new Subject<Unit>();
        private Subject<Unit> onStoppedSubject = new Subject<Unit>();
        private Subject<Unit> onEndedSubject = new Subject<Unit>();

        /// <summary> Raises whenever <see cref="Play"/> is called. </summary>
        public IObservable<Unit> OnPlayedAsObservable => onPlayedSubject.AsObservable();
        /// <summary> Raises whenever <see cref="Pause"/> is called. </summary>
        public IObservable<Unit> OnPausedAsObservable => onPausedSubject.AsObservable();
        /// <summary> Raises whenever <see cref="Resume"/> is called. </summary>
        public IObservable<Unit> OnResumedAsObservable => onResumedSubject.AsObservable();
        /// <summary> Raises whenever <see cref="Stop"/> is called. </summary>
        public IObservable<Unit> OnStoppedAsObservable => onStoppedSubject.AsObservable();
        /// <summary> Raises when the non-looping sequence animation is ended. </summary>
        public IObservable<Unit> OnEndedAsObservable => onEndedSubject.AsObservable();

        private int frame;
        private float currentTime;
        private int loopTimes;

        public bool IsPlaying { get; private set; }

        public void Play()
        {
            if (!ValidateSprites(sprites)) return;

            ResetSequenceAnimation();

            IsPlaying = true;

            onPlayedSubject.OnNext(Unit.Default);

            serialDisposable.Disposable ??= Observable.EveryUpdate()
            .Subscribe(OnNext);
        }

        public void Pause()
        {
            if (!ValidateSprites(sprites)) return;

            IsPlaying = false;

            onPausedSubject.OnNext(Unit.Default);

            serialDisposable.Disposable ??= Observable.EveryUpdate()
            .Subscribe(OnNext);
        }

        public void Resume()
        {
            if (!ValidateSprites(sprites)) return;

            IsPlaying = true;

            onResumedSubject.OnNext(Unit.Default);

            serialDisposable.Disposable ??= Observable.EveryUpdate()
            .Subscribe(OnNext);
        }

        public void Stop()
        {
            if (!ValidateSprites(sprites)) return;

            ResetSequenceAnimation();

            IsPlaying = false;

            onStoppedSubject.OnNext(Unit.Default);

            serialDisposable.Disposable ??= Observable.EveryUpdate()
            .Subscribe(OnNext);
        }

        private void ResetSequenceAnimation()
        {
            frame = 0;
            SetSprite(sprites[frame]);

            currentTime = 0f;
            loopTimes = 0;
        }

        private void OnNext(long _)
        {
            if (!IsPlaying) return;

            if (loops != -1 && loopTimes >= loops)
            {
                IsPlaying = false;

                onEndedSubject.OnNext(Unit.Default);

                return;
            }

            currentTime += Time.deltaTime;
            float secondPerFrame = 1f / fps;

            while (currentTime >= secondPerFrame)
            {
                SetSprite(sprites[frame]);
                frame++;
                if (frame >= sprites.Length)
                {
                    frame = 0;

                    if (loops != -1)
                        loopTimes++;
                }

                currentTime -= secondPerFrame;
            }
        }

        protected abstract void SetSprite(Sprite sprite);

        private void OnEnable()
        {
            switch (onEnableBehavior)
            {
                case OnEnableBehavior.None:
                    break;
                case OnEnableBehavior.Play:
                    Play();
                    break;
                case OnEnableBehavior.Resume:
                    Resume();
                    break;
            }
        }

        private void OnDisable()
        {
            switch (onDisableBehavior)
            {
                case OnDisableBehavior.None:
                    break;
                case OnDisableBehavior.Pause:
                    Pause();
                    break;
                case OnDisableBehavior.Stop:
                    Stop();
                    break;
            }
        }

        private void OnDestroy()
        {
            serialDisposable.Disposable?.Dispose();
            serialDisposable.Disposable = null;

            onPlayedSubject.OnCompleted();
            onPausedSubject.OnCompleted();
            onResumedSubject.OnCompleted();
            onStoppedSubject.OnCompleted();
            onEndedSubject.OnCompleted();
        }
    }
}
