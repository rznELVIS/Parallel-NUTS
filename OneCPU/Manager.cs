namespace OneCPU
{
    using System;

    public class Manager
    {
        /// <summary>
        /// Амлитуда.
        /// </summary>
        private double _a;

        /// <summary>
        /// Масса.
        /// </summary>
        private double _m;

        /// <summary>
        /// Время одного колебания.
        /// </summary>
        private double _period;

        private double _phi;


        public void Exceute()
        {
            this._a = 11;
            this._m = 3;
            this._period = 2;
            this._phi = 2;

            int globalWorkSize = 102400000;
            var result = new double [globalWorkSize];

            for (var i = 0; i < globalWorkSize; i++)
            {
                result[i] = this.GetKintcickValue(i);
            }
        }

        private double GetKintcickValue(double t)
        {
            var res = 2 * Math.PI * Math.PI;
            res = res * this._a * this._a;
            res = res / (this._period * this._period);

            res *= res * this._m;

            var value = 2 * Math.PI / this._period;
            value = value * t;
            value += this._phi;

            res = res * Math.Cos(value) * Math.Cos(value);

            return res;
        }
    }
}
