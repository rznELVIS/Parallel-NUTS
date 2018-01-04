__kernel void Calculate(__global float* a, __constant float* coeff)
{
	int iGID = get_global_id(0);

	//kitetic
	float resK = 2 *  M_PI * M_PI;
	resK = resK * coeff[0] * coeff[0];
	resK = resK / (coeff[2] * coeff[2]);

	resK = resK * coeff[1];

	float value = 2 * M_PI / coeff[2];
	value = value * iGID;
	value = value + coeff[3];

	resK = resK * cos(value) * cos(value);

	//potential
	float resP = 2 *  M_PI * M_PI;
	resP = resP * coeff[0] * coeff[0];
	resP = resP / (coeff[2] * coeff[2]);

	resP = resP * coeff[1];

	value = 2 * M_PI / coeff[2];
	value = value * iGID;
	value = value + coeff[3];

	resP = resP * sin(value) * sin(value);

	a[iGID] = resK;
}