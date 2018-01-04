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

            this.DisplayDeviceInfo(gpuDevice);

            int localWorkSize = 1024;
            int globalWorkSize = localWorkSize * 10000;
            
            float[] srcA = new float[globalWorkSize];

            using (var context = Context.Create(gpuDevice[0]))
            {
                using (CommandQueue commandQueue = context.CreateCommandQueue(gpuDevice[0], CommandQueueProperties.None))
                {
                    using (Buffer bufferA = context.CreateBuffer(MemoryFlags.ReadWrite, srcA.Length * sizeof(float)))
                    {
                        string path = @"d:\Study\NUTS\GPU\Parallel-NUTS\OpenCl\kernels\example.cl";
                        var openClProgram = new OpenClProgram(context);

                        string options = "-cl-fast-relaxed-math";

                        using (var program = openClProgram.LoadFromFile(path, options))
                        {
                            using (Kernel kernel = program.CreateKernel("VectorAdd"))
                            {
                                kernel.Arguments[0].SetValue(bufferA);
                                kernel.Arguments[1].SetValue(2);

                                fixed (float* psrcA = srcA)
                                {
                                    using (commandQueue.EnqueueWriteBuffer(bufferA, false, 0, sizeof(float) * globalWorkSize, (IntPtr)psrcA))
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

        private void DisplayDeviceInfo(Device[] devices)
        {
            foreach (var device in devices)
            {
                Console.WriteLine($"Name: {device.Name}");
                Console.WriteLine($"Max work group size: {device.MaxWorkGroupSize}");
                Console.WriteLine($"OpenCL version: {device.OpenCLVersion}");
                Console.WriteLine($"Version: {device.Version}");

                Console.WriteLine("Extensions:");
                foreach (var extension in device.Extensions)
                {
                    Console.WriteLine(extension);
                }

                Console.WriteLine("-------");
            }
        }
    }
}
