using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Profiling;

namespace Atuvu.Pooling
{
    static class EventByteConverter
    {
        static readonly Dictionary<Type, byte[]> s_Buffers = new Dictionary<Type, byte[]>();

        public static byte[] ToByte<T>(T evt) where T : struct, IPoolProfilerEvent<T>
        {
            using (new ProfilerMarker("Event To Byte Convertion").Auto())
            {
                byte[] buffer = GetBufferForEvent<T>();
                int size = buffer.Length;

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(evt, ptr, true);
                Marshal.Copy(ptr, buffer, 0, size);
                Marshal.FreeHGlobal(ptr);

                return buffer;
            }
        }

        public static T FromByte<T>(byte[] raw) where T : struct, IPoolProfilerEvent<T>
        {
            using (new ProfilerMarker("Byte To Event Convertion").Auto())
            {
                byte[] buffer = GetBufferForEvent<T>();
                int size = buffer.Length;

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, 0, ptr, size);
                T evt = (T)Marshal.PtrToStructure(ptr, typeof(T));
                Marshal.FreeHGlobal(ptr);

                return evt;
            }
        }

        static byte[] GetBufferForEvent<T>() where T : struct, IPoolProfilerEvent<T>
        {
            if(!s_Buffers.TryGetValue(typeof(T), out byte[] buffer))
            {
                buffer = new byte[Marshal.SizeOf<T>()];
                s_Buffers.Add(typeof(T), buffer);
            }

            return buffer;
        }
    }
}