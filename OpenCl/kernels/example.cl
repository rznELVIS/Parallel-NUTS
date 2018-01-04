__kernel void VectorAdd(__global float* a, float b)
{
    int iGID = get_global_id(0); 
    a[iGID] = iGID + b;
} 