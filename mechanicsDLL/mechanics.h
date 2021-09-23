#pragma once

#define CALCULATION_API __declspec(dllexport)

extern "C" CALCULATION_API float optimisation_obj_res(float* hardpoints, float wRadiusin, 
	float wheelbase, float cogHeight, float frontDriveBias, float frontBrakeBias,
	int suspPos, int drivePos, int brakePos,
	float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin, float *outputParams);

extern "C" CALCULATION_API void test_py(double *data, double *data2);
