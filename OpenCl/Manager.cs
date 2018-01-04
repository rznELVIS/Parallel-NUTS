namespace OpenCl.NOPenCL
{
    using System;
    using System.Diagnostics;

    using NOpenCL;

    using OpenCl.NOPenCL.Execution;

    using Buffer = NOpenCL.Buffer;

    public class Manager
    {
        public unsafe void Execute()
        {
            var sw = new Stopwatch();
            sw.Start();

            Platform platform = Platform.GetPlatforms()[0];

            var gpuDevice = platform.GetDevices(DeviceType.Gpu);

            this.DisplayDeviceInfo(gpuDevice);

            int localWorkSize = 1024;
            int globalWorkSize = localWorkSize * 10000;
            
            float[] srcA = new float[globalWorkSize];
            float[] coeff = new float[4];

            coeff[0] = 11;
            coeff[1] = 3;
            coeff[2] = 2;
            coeff[3] = 2;

            using (var context = Context.Create(gpuDevice[0]))
            {
                using (CommandQueue commandQueue = context.CreateCommandQueue(gpuDevice[0], CommandQueueProperties.None))
                {
                    using (Buffer bufferA = context.CreateBuffer(MemoryFlags.ReadWrite, srcA.Length * sizeof(float)))
                    using (Buffer bufferCoeff = context.CreateBuffer(MemoryFlags.ReadOnly, coeff.Length * sizeof(float)))
                    {
                        string path = @"d:\Study\NUTS\GPU\Parallel-NUTS\OpenCl\kernels\example.cl";
                        var openClProgram = new OpenClProgram(context);

                        string options = "-cl-fast-relaxed-math";

                        using (var program = openClProgram.LoadFromFile(path, options))
                        {
                            using (Kernel kernel = program.CreateKernel("Calculate"))
                            {
                                sw.Stop();
                                Console.WriteLine($"Infrastructure time: {sw.Elapsed.TotalSeconds} sec.");

                                sw.Restart();
                                for (int i = 0; i < 100; i++)
                                {
                                    kernel.Arguments[0].SetValue(bufferA);
                                    kernel.Arguments[1].SetValue(bufferCoeff);

                                    fixed (float* psrcA = srcA)
                                    fixed (float* psrcCoeff = coeff)
                                    {
                                        using (commandQueue.EnqueueWriteBuffer(bufferA, false, 0, sizeof(float) * globalWorkSize, (IntPtr)psrcA))
                                        {
                                        }

                                        using (commandQueue.EnqueueWriteBuffer(bufferCoeff, false, 0, sizeof(float) * coeff.Length, (IntPtr)psrcCoeff))
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

                                sw.Stop();
                                Console.WriteLine($"Compute time: {sw.Elapsed.TotalSeconds} sec.");
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
