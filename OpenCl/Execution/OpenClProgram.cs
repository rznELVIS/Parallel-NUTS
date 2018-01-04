namespace OpenCl.NOPenCL.Execution
{
    using System;
    using System.IO;

    using NOpenCL;

    public class OpenClProgram
    {
        private Context _context;

        public OpenClProgram(Context context)
        {
            this._context = context;
        }

        public Program LoadFromFile(string path, string options)
        {
            using (var sr = new StreamReader(path))
            {
                string text = sr.ReadToEnd();
                return this.Load(text, options);
            }
        }

        private Program Load(string source, string options)
        {
            Program program = _context.CreateProgramWithSource(source);
            program.Build(options);

            return program;
        }
    }
}
