// testExe.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <Eigen/Dense>
#include <Eigen/Geometry>
#include <iostream>

int main()
{

	auto aSlope = [](Eigen::Vector3f& ca1, Eigen::Vector3f& ca2, Eigen::Vector3f& ca3)  // ca short for control arm
	{
		float aCA =
			((ca1(0) - ca2(0)) * (ca1(1) - ca3(1)) - (ca1(0) - ca3(0)) * (ca1(1) - ca2(1))) /
			((ca1(0) - ca2(0)) * (ca1(2) - ca3(2)) - (ca1(0) - ca3(0)) * (ca1(2) - ca2(2)));
		std::cout << ca3(0) << " slope vector\n";
		return aCA;
	};

	Eigen::Vector3f ca1{1.1,2.3,3.2};
	Eigen::Vector3f ca2{11.161,12.4,13.8};
	Eigen::Vector3f ca3{21.61,22.9618,23.6132};

	float a1{ 1.0f };
	float a2{ 1.0f };
	float a3{ 1.0f };

	auto aSlope2 = [](float ca1, float ca2, float ca3)  // ca short for control arm
	{
		float aCA =	ca1 + ca2 + ca3;
		return aCA;
	};

	float a = (float)aSlope(ca1,ca2,ca3);
	std::cout << a << " slope\n";

	float a12 = aSlope2(a1, a2, a3);
	std::cout << a12 << " slope\n";
    std::cout << "Hello World!\n";
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
