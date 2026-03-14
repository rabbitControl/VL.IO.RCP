 
using System;
using System.IO;

namespace RCP.IO
{
    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

    public static partial class SystemIOExtensions
    {
        public static void WriteValue(this BinaryWriter writer, byte[] value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
            {
                for (int i = value.Length - 1; i > -1; i--)
                    writer.Write(value[i]);
            }
        }

        public static void WriteValues(this BinaryWriter writer, byte[] values, int fieldSize, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(values);
            else
            {
                for (int i = 0; i < values.Length; i += fieldSize)
                  for (int j = i + fieldSize - 1; j >= i; j--)
                      writer.Write(values[j]);
            }
        }

        public static void Write(this BinaryWriter writer, Boolean value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Boolean[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(Boolean)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(Boolean), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Int16 value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Int16[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(Int16)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(Int16), byteOrder);
        }

        public static void Write(this BinaryWriter writer, UInt16 value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, UInt16[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(UInt16)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(UInt16), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Int32 value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Int32[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(Int32)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(Int32), byteOrder);
        }

        public static void Write(this BinaryWriter writer, UInt32 value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, UInt32[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(UInt32)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(UInt32), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Int64 value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Int64[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(Int64)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(Int64), byteOrder);
        }

        public static void Write(this BinaryWriter writer, UInt64 value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, UInt64[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(UInt64)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(UInt64), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Single value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Single[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(Single)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(Single), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Double value, ByteOrder byteOrder)
        {
            if (byteOrder == ByteOrder.LittleEndian)
                writer.Write(value);
            else
                writer.WriteValue(BitConverter.GetBytes(value), byteOrder);
        }

        public static void Write(this BinaryWriter writer, Double[] buffer, int offset, int count, ByteOrder byteOrder = ByteOrder.LittleEndian)
        {
            var byteBuffer = new byte[count * sizeof(Double)];
            Buffer.BlockCopy(buffer, offset, byteBuffer, 0, byteBuffer.Length);
            writer.WriteValues(byteBuffer, sizeof(Double), byteOrder);
        }
    }
}
 
