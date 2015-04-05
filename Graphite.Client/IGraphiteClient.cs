using System;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client
{
    /// <summary>
    /// Интерфейс, позволяющий отправлять точки напрямую в графит
    /// </summary>
    [PublicAPI]
    public interface IGraphiteClient
    {
        /// <summary>
        /// Отправка одной точки в графит
        /// </summary>
        /// <param name="path">Имя метрики (например, MyProject.MyService.ProcessorTime)</param>
        /// <param name="value">Значение точки</param>
        /// <param name="timestamp">Время точки, всегда будет сконвертировано в UTC и отправлено в графит в виде unixtimstamp'а</param>
        void Send([NotNull] string path, long value, DateTime timestamp);
    }
}