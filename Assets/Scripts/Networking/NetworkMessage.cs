using System.IO;
using System;

public class NetworkMessage {
    public ushort length { get; set; }
    public byte[] content { get; set; }

    public static NetworkMessage ReadFromStream(BinaryReader reader) {
        ushort len;
        byte[] len_buf;
        byte[] buffer;

        len_buf = reader.ReadBytes(2);
        if (BitConverter.IsLittleEndian) {
            Array.Reverse(len_buf);
        }
        len = BitConverter.ToUInt16(len_buf, 0);

        buffer = reader.ReadBytes(len);

        return new NetworkMessage(buffer);
    }

    public void WriteToStream(BinaryWriter writer) {
        byte[] len_bytes = BitConverter.GetBytes(length);

        if (BitConverter.IsLittleEndian) {
            Array.Reverse(len_bytes);
        }
        writer.Write(len_bytes);

        writer.Write(content);
    }

    public string ToString() {
        return System.Text.Encoding.UTF8.GetString(content);
    }

    public NetworkMessage(byte[] data) {
        content = data;
        length = (ushort) data.Length;
    }

    public static NetworkMessage FromString(string data) {
        return new NetworkMessage(System.Text.Encoding.UTF8.GetBytes(data));
    }
}
