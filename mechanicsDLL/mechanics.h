#pragma once

#define CALCULATION_API __declspec(dllexport)

extern "C" CALCULATION_API double optimisation_obj_res(float* hardpoints, float wRadiusin, float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin);

extern "C" CALCULATION_API void test_py(double *data, double *data2);
