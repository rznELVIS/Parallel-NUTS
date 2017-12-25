namespace OpenCl.NOPenCL
{
    using System;

    using NOpenCL;

    using Buffer = NOpenCL.Buffer;

    public class Manager
    {
        public unsafe void Execute()
        {
            Platform platform = Platform.GetPlatforms()[0];

            var gpuDevice = platform.GetDevices(DeviceType.Gpu);

            int globalWorkSize = 1024;
            int localWorkSize = 1024;
            float[] srcA = new float[globalWorkSize];

            using (var context = Context.Create(gpuDevice[0]))
            {
                using (CommandQueue commandQueue = context.CreateCommandQueue(gpuDevice[0], CommandQueueProperties.None))
                {
                    using (Buffer bufferA = context.CreateBuffer(MemoryFlags.ReadWrite, 1024 * sizeof(float)))
                    {
                        string source = @"__kernel void VectorAdd(__global float* a){ a[0] = 1; }";
                        using (NOpenCL.Program program = context.CreateProgramWithSource(source))
                        {
                            string options;
#if false
                            options = "-cl-fast-relaxed-math -DMAC";
#else
                            options = "-cl-fast-relaxed-math";
#endif
                            program.Build(options);

                            using (Kernel kernel = program.CreateKernel("VectorAdd"))
                            {
                                kernel.Arguments[0].SetValue(bufferA);

                                fixed (float* psrcA = srcA)
                                {
                                    using (commandQueue.EnqueueWriteBuffer(bufferA, false, 0, sizeof(float) * 1024, (IntPtr)psrcA))
                                    {
                                    }

                                    using (commandQueue.EnqueueNDRangeKernel(kernel, (IntPtr)globalWorkSize, (IntPtr)localWorkSize))
                                    {
                                    }

                                    // synchronous/blocking read of results, and check accumulated errors
                                    using (commandQueue.EnqueueReadBuffer(bufferA, true, 0, sizeof(float) * globalWorkSize, (IntPtr)psrcA))
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
