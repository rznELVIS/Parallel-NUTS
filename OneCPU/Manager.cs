namespace OneCPU
{
    using System;

    public class Manager
    {
        /// <summary>
        /// Амлитуда.
        /// </summary>
        private float _a;

        /// <summary>
        /// Масса.
        /// </summary>
        private float _m;

        /// <summary>
        /// Время одного колебания.
        /// </summary>
        private float _period;

        /// <summary>
        /// Сдвиг по фазе.
        /// </summary>
        private float _phi;

        public void Exceute()
        {
            this._a = 11;
            this._m = 3;
            this._period = 2;
            this._phi = 2;

            // 128000000 max for double
            int globalWorkSize = 128000000;
            //float[] resultK = new float[globalWorkSize];
            //float[] resultP = new float[globalWorkSize];

            for (var i = 0; i < globalWorkSize; i++)
            {
                var valueK = this.GetKinectickValue(i);
                var valueP = this.GetPotentialValue(i);
            }
        }

        /// <summary>
        /// http://www.yaklass.ru/materiali?mode=cht&chtid=90
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private float GetKinectickValue(double t)
        {
            var res = 2 * Math.PI * Math.PI;
            res = res * this._a * this._a;
            res = res / (this._period * this._period);

            res = res * this._m;

            var value = 2 * Math.PI / this._period;
            value = value * t;
            value += this._phi;

            res = res * Math.Cos(value) * Math.Cos(value);

            return (float)res;
        }

        private double GetPotentialValue(double t)
        {
            var res = 2 * Math.PI * Math.PI;
            res = res * this._a * this._a;
            res = res / (this._period * this._period);

            res = res * this._m;

            var value = 2 * Math.PI / this._period;
            value = value * t;
            value += this._phi;

            res = res * Math.Sin(value) * Math.Sin(value);

            return (float)res;
        }
    }
}
