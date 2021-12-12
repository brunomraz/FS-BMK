#pragma once

#define CALCULATION_API __declspec(dllexport)

extern "C" CALCULATION_API void optimisation_obj_res(float* hardpoints, int suspPos, float wRadius,
	float wheelbase, float cogHeight, float driveBias, float brakeBias,
	int drivePos, int brakePos,
	float wVert, float* peakWidth, float* flatness, float wSteer, int vertIncr, int steerIncr, float precision, float* targetValues,
	float* weightFactors, float& obj_func_res, float* outputParams);


extern "C" CALCULATION_API void suspension_movement(float* hardpoints, float wRadiusin,
	float wheelbase, float cogHeight, float frontDriveBias, float frontBrakeBias,
	int suspPos, int drivePos, int brakePos,
	float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin,

	float* camberAngle, float* toeAngle, float* casterAngle, float* rcHeight, float* casterTrail,
	float* scrubRadius, float* kingpinAngle, float* antiDrive, float* antiBrake, float* halfTrackChange,
	float* wheelbaseChange,
	float* constOutputParams,

	float* outputLca3, float* outputUca3, float* outputTr1, float* outputTr2, float* outputWcn, float* outputSpn);