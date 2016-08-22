using System;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client
{
    /// <summary>
    ///     Интерфейс, позволяющий отправлять аннотации
    /// </summary>
    [PublicAPI]
    public interface IGraphiteAnnotationsClient
    {
        /// <summary>
        ///     Отправка аннотации
        /// </summary>
        /// <param name="title">Заголовок аннотации</param>
        /// <param name="tags">Тэги аннотации</param>
        void PostEvent([NotNull] string title, [CanBeNull] string[] tags);

        /// <summary>
        ///     Отправка аннотации
        /// </summary>
        /// <param name="title">Заголовок аннотации</param>
        /// <param name="tags">Тэги аннотации</param>
        /// <param name="utcTimestamp">Время по UTC в формате Epoch time</param>
        void PostEvent([NotNull] string title, [CanBeNull] string[] tags, long utcTimestamp);

        /// <summary>
        ///     Отправка аннотации
        /// </summary>
        /// <param name="title">Заголовок аннотации</param>
        /// <param name="tags">Тэги аннотации</param>
        /// <param name="utcDateTime">Время по UTC</param>
        void PostEvent([NotNull] string title, [CanBeNull] string[] tags, DateTime utcDateTime);

    }
}