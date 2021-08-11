//#pragma once
//
//#define CALCULATION_API __declspec(dllexport)
//
//
//extern "C" CALCULATION_API double addition(double a, double b);
//



#pragma once

#define CALCULATION_API __declspec(dllexport)

extern "C" CALCULATION_API double addition(double a, double b);
