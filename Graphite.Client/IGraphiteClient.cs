using System;

namespace SKBKontur.Graphite.Client
{
    public interface IGraphiteClient
    {
        /// <summary>
        /// �������� ����� ����� � ������
        /// </summary>
        /// <param name="path">��� ������� (��������, MyProject.MyService.ProcessorTime)</param>
        /// <param name="value">�������� �����</param>
        /// <param name="timestamp">����� �����</param>
        void Send(string path, int value, DateTime timestamp);
    }
}