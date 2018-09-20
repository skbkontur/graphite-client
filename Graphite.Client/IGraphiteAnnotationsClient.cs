using System;
using System.Net.Http;

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
        [CanBeNull]
        HttpResponseMessage PostEvent([NotNull] string title, [CanBeNull, ItemNotNull] string[] tags);

        /// <summary>
        ///     Отправка аннотации
        /// </summary>
        /// <param name="title">Заголовок аннотации</param>
        /// <param name="tags">Тэги аннотации</param>
        /// <param name="utcTimestamp">Время по UTC в формате Epoch time</param>
        [CanBeNull]
        HttpResponseMessage PostEvent([NotNull] string title, [CanBeNull, ItemNotNull] string[] tags, long utcTimestamp);

        /// <summary>
        ///     Отправка аннотации
        /// </summary>
        /// <param name="title">Заголовок аннотации</param>
        /// <param name="tags">Тэги аннотации</param>
        /// <param name="utcDateTime">UTC метка времени</param>
        [CanBeNull]
        HttpResponseMessage PostEvent([NotNull] string title, [CanBeNull, ItemNotNull] string[] tags, DateTime utcDateTime);
    }
}