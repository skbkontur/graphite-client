using System;

namespace SKBKontur.Graphite.Client
{
    public interface IGraphiteClient
    {
        /// <summary>
        /// Отправка одной точки в графит
        /// </summary>
        /// <param name="path">Имя метрики (например, MyProject.MyService.ProcessorTime)</param>
        /// <param name="value">Значение точки</param>
        /// <param name="timestamp">Время точки</param>
        void Send(string path, int value, DateTime timestamp);
    }
}