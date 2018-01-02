__kernel void VectorAdd(__global float* a)
{ 
    int iGID = get_global_id(0); 
    a[iGID] = iGID;
} 