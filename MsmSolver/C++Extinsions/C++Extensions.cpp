// C++Extinsions.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
using namespace concurrency;
using namespace std;

extern "C" __declspec ( dllexport ) void MatMul(double* vC,double* vA,
 double* vB, int M, int N, int W)
{
    array_view<const double, 2> a(M, W, vA), b(W, N, vB);
	array_view<double, 2> c(M, N, vC);
	c.discard_data();
	parallel_for_each(c.extent, [=](index<2> idx) restrict(amp)
	{
	  int row = idx[0]; int col = idx[1];
	  double sum = 0;
	  for(int i = 0; i < b.extent[0]; i++)
		sum += a(row, i) * b(i, col);
	  c[idx] = sum;
	});
	c.synchronize();
}

//extern "C" __declspec ( dllexport ) void MatMulFire(double* vC,double* vA,
// double* vB, int M, int N, int W)
//{
//	af::array A = af::array(M,W,vA,af::afHost,0);
//	af::array B = af::array(W,N,vB,af::afHost,0);
//	af::array C = matmul(A,B);
//	C.eval();
//	vC = C.host<double>();
//}


//extern "C" __declspec ( dllexport ) void MatVectMul(double* vC,double* vA,
// double* vB, int M, int W)
//{
//    array_view<const double, 2> a(M, W, vA);
//	array_view<double, 1> c(M, vC), b(W, vB);
//	c.discard_data();
//	parallel_for_each(c.extent, [=](index<2> idx) restrict(amp)
//	{
//	  int row = idx[0]; int col = idx[1];
//	  double sum = 0;
//	  for(int i = 0; i < b.extent[0]; i++)
//		sum += a(row, i) * b(i);
//	  c[idx[0]] = sum;
//	});
//	c.synchronize();
//}

//extern "C" __declspec ( dllexport ) void VectMul(double* vC,double* mA,
// double* vB, int M, int N, int W)
//{
//	array_view<const double, 2> a(M, W, mA), b(W, N, vB);
//	array_view<double, 2> c(M, N, vC);
//	c.discard_data();
//	parallel_for_each(c.extent, [=](index<2> idx) restrict(amp)
//	{
//	  int row = idx[0]; int col = idx[1];
//	  double sum = 0;
//	  for(int i = 0; i < b.extent[0]; i++)
//		sum += a(row, i) * b(i, col);
//	  c[idx] = sum;
//	});
//	c.synchronize();
//}

extern "C" __declspec ( dllexport ) void TilingMultiplication(double* vA,
				double* vB,double* vC, int M, int N, int W, const int ts )
  {
	const static int TS = 32;
	
	array_view<const double,2> a(M, W, vA), b(W, N, vB);
    array_view<double,2> c(M, N, vC);  
    c.discard_data();

    parallel_for_each(c.extent.tile<TS,TS>(),
       [=](tiled_index<TS,TS> t_idx) restrict(amp) 
    {
      int row = t_idx.local[0]; int col = t_idx.local[1];
     tile_static double locA[TS][TS], locB[TS][TS];
     double sum = 0;
     for (int i = 0; i < a.extent[1]; i += TS) {
       locA[row][col] = a(t_idx.global[0], col + i);
       locB[row][col] = b(row + i, t_idx.global[1]);
       t_idx.barrier.wait();

       for (int k = 0; k < TS; k++)
         sum += locA[row][k] * locB[k][col];           
       t_idx.barrier.wait();
     }
     c[t_idx.global] = sum;
   });
   c.synchronize();
 }


extern "C" _declspec ( dllexport ) const wchar_t* InitializeLibrary ()
{
	std::vector<accelerator> accs = accelerator::get_all();
	
	if (accs.size()>0)
		return accs[0].description.c_str();
	else return nullptr;
}


//extern "C" __declspec ( dllexport ) void MatMulCPU(double* vC,double* vA,
// double* vB, int M, int N, int W)
//{
//    array_view<const double, 2> a(M, W, vA), b(W, N, vB);
//	array_view<double, 2> c(M, N, vC);
//	c.discard_data();
//	parallel_for_each(c.extent, [=](index<2> idx) restrict(amp,cpu)
//	{
//	  int row = idx[0]; int col = idx[1];
//	  double sum = 0;
//	  for(int i = 0; i < b.extent[0]; i++)
//		sum += a(row, i) * b(i, col);
//	  c[idx] = sum;
//	});kernel32.lib;user32.lib;gdi32.lib;winspool.lib;comdlg32.lib;advapi32.lib;shell32.lib;ole32.lib;oleaut32.lib;uuid.lib;odbc32.lib;odbccp32.lib;%(AdditionalDependencies)
       //kernel32.lib; user32.lib; gdi32.lib; winspool.lib; comdlg32.lib; advapi32.lib; shell32.lib; ole32.lib; oleaut32.lib; uuid.lib; odbc32.lib; odbccp32.lib; % (AdditionalDependencies)
//	c.synchronize();
//}

extern "C" _declspec ( dllexport ) std::wstring ShowAccelerator ()
{
	std::wstring result = std::wstring();
	std::vector<accelerator> accs = accelerator::get_all();
	for (int i = 0; i < accs.size(); i++)
	{
		result+=accs[i].description;
		result+=+'/n';
	}
	return result;
}
//
//extern "C" __declspec ( dllexport ) void _stdcall square_array(float* arr, int n)
//{
//  // Create a view over the data on the CPU
//  array_view<float,1> dataView(n, &arr[0]);
//
//  // Run code on the GPU
//  parallel_for_each(dataView.grid, [=] (index<1> idx) mutable restrict(direct3d)
//  {
//    dataView[idx] = dataView[idx] * dataView[idx];
//  });
//
//  // Copy data from GPU to CPU
//  dataView.synchronize();
//}