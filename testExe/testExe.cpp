// testExe.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <Eigen/Dense>
#include <Eigen/Geometry>
#include <iostream>


template <typename T> int sgn(T val) {
	return (T(0) < val) - (val < T(0));
}

template <typename T>
T ObjFuncModule(T variable, T target, T weightFactor, T peakWidth, T flatness)
{
	T objfunc = weightFactor * (T)exp(-1 / peakWidth * pow(abs(variable - target), flatness));
	std::cout << objfunc << "\n";
	return objfunc;

}

float* ReturnArray(int n)
{
	float someArray[5];
	for (int i = 0; i < 5; i++)
	{
		someArray[i] = i * n;
	}

	return someArray;
}

int main()
{

	std::cout << "camber down" << "\n";
	float camberDown = ObjFuncModule(-2.900272131, -2.6500000953674316, 0.4032258093357086, 0.10000000149011612, 2.0);

	std::cout << "camber up" << "\n";
	float camberUp = ObjFuncModule(-0.664889454841613, -0.949999988079071, 0.4032258093357086, 0.10000000149011612, 2.0);

	std::cout << "toe down" << "\n";
	float toeDown = ObjFuncModule(-0.0740188062191009, 0.0, 0.02016128972172737, 10.0, 2.0);

	std::cout << "toe up" << "\n";
	float toeUp = ObjFuncModule(0.0, 0.0, 0.02016128972172737 ,10.0, 2.0);

	std::cout << "caster angle" << "\n";
	float casterAngle = ObjFuncModule(4.593552113, 7.0, 0.012096770107746124, 0.10000000149011612, 3.0);

	std::cout <<"RC height" << "\n";
	float rcHeight = ObjFuncModule(49.9998703002929, 50.0, 0.008064515888690948, 10.0, 2.0);

	std::cout << "Caster trail" << "\n";
	float casterTrail = ObjFuncModule(19.3047523498535, 15.0, 0.02016128972172737, 200000.0, 6.0);

	std::cout << "scrub radius" << "\n";
	float scrubRadius = ObjFuncModule(-7.8126482963562, -7.0, 0.008064515888690948, 10.0, 2.0);

	std::cout << "kingpin angle" << "\n";
	float kingpinAngle = ObjFuncModule(7.72390794754028, 4.0, 0.008064515888690948 ,10.0 ,2.0);

	std::cout << "anti drive" << "\n";
	float antiDrive = ObjFuncModule(10.0186567306518, 18.0, 0.008064515888690948, 10.0, 2.0);
	
	std::cout << "anti brake" << "\n";
	float antiBrake = ObjFuncModule(3.6288845539093, 10.0, 0.008064515888690948 ,10.0, 2.0);

	std::cout << "half track change down" << "\n";
	float halftrackDown = ObjFuncModule(-4.34222412109375, 0.0, 0.008064515888690948, 10.0, 2.0);

	std::cout << "half track change up" << "\n";
	float halftrackUp = ObjFuncModule(0.4884033203125, 0.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "wheelbase change down" << "\n";
	float wheelbaseDown = ObjFuncModule(0.537353515625, 0.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "wheelbase change up" << "\n";
	float wheelbaseUp = ObjFuncModule(-0.42724609375, 0.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "LCA3 free radius" << "\n";
	float LCA3freeRadius = ObjFuncModule(79.2653045654296, 100.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "UCA3 free radius" << "\n";
	float UCA3freeRadius = ObjFuncModule(91.783103942871, 100.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "TR2 free radius" << "\n";
	float TR2freeRadius = ObjFuncModule(69.768310546875, 100.0, 0.008064515888690948, 10.0, 2.0);

	std::cout << "LCA3 WCN distance" << "\n";
	float LCA3WCNDist = ObjFuncModule(-21.5946617126464, -20.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "UCA3 WCN distance" << "\n";
	float UCA3WCNDist = ObjFuncModule(-39.661334991455, -20.0 ,0.008064515888690948 ,10.0 ,2.0);

	std::cout << "TR2 WCN distance" << "\n";
	float TR2WCNDist = ObjFuncModule(-30.5479946136474, -20.0, 0.008064515888690948 ,10.0 ,2.0);


	std::cout << "obj func res" << "\n";
	float objfuncres = 1 - camberDown - camberUp - toeDown - toeUp - casterAngle - rcHeight - casterTrail -
		scrubRadius - kingpinAngle - antiDrive - antiBrake - halftrackDown - halftrackUp - wheelbaseDown -
		wheelbaseUp - LCA3freeRadius - UCA3freeRadius - TR2freeRadius - LCA3WCNDist - UCA3WCNDist - TR2WCNDist;
	std::cout << objfuncres;


	std::cin;

}


// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
