namespace OpenCl.NOPenCL
{
    using System;

    using NOpenCL;

    using OpenCl.NOPenCL.Execution;

    using Buffer = NOpenCL.Buffer;

    public class Manager
    {
        public unsafe void Execute()
        {
            Platform platform = Platform.GetPlatforms()[0];

            var gpuDevice = platform.GetDevices(DeviceType.Gpu);

            int globalWorkSize = 10240;
            int localWorkSize = 1024;
            float[] srcA = new float[globalWorkSize];

            using (var context = Context.Create(gpuDevice[0]))
            {
                using (CommandQueue commandQueue = context.CreateCommandQueue(gpuDevice[0], CommandQueueProperties.None))
                {
                    using (Buffer bufferA = context.CreateBuffer(MemoryFlags.ReadWrite, srcA.Length * sizeof(float)))
                    {
                        string source = @"__kernel void VectorAdd(__global float* a){ int iGID = get_global_id(0); a[iGID] = iGID; }";
                        var openClProgram = new OpenClProgram(context);

                        string options = "-cl-fast-relaxed-math";

                        using (var program = openClProgram.Load(source, options))
                        {
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
