using System;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client
{
    /// <summary>
    /// ���������, ����������� ���������� ����� �������� � ������
    /// </summary>
    [PublicAPI]
    public interface IGraphiteClient
    {
        /// <summary>
        /// �������� ����� ����� � ������
        /// </summary>
        /// <param name="path">��� ������� (��������, MyProject.MyService.ProcessorTime)</param>
        /// <param name="value">�������� �����</param>
        /// <param name="timestamp">����� �����, ������ ����� ��������������� � UTC � ���������� � ������ � ���� unixtimstamp'�</param>
        void Send([NotNull] string path, long value, DateTime timestamp);
    }
}