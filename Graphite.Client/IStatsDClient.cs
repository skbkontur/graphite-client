﻿using System;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client
{
    /// <summary>
    ///     Интерфейс, позволяющий отправлять точки в StatsD backend
    /// </summary>
    [PublicAPI]
    public interface IStatsDClient : IDisposable
    {
        /// <summary>
        ///     Метод для отправки таймингов в StatsD.
        ///     Отправляет сообщение key:value|ms
        /// </summary>
        /// <param name="value">Значение метрики</param>
        /// <param name="sampleRate">Вероятность отправки значения по сети</param>
        /// <param name="keys">Ключи, по которым будет учитывается метрика</param>
        void Timing(long value, double sampleRate, [NotNull] params string[] keys);

        /// <summary>
        ///     Метод для отправки счётчиков в StatsD.
        ///     Отправляет сообщения вида key:value|c
        /// </summary>
        /// <param name="magnitude">Значение инкремента для счётчикка. Может быть отричательным</param>
        /// <param name="sampleRate">Вероятность отправки значения по сети</param>
        /// <param name="keys">Ключи, по которым будет учитывается метрика</param>
        void Increment(int magnitude, double sampleRate, [NotNull] params string[] keys);
    }
}