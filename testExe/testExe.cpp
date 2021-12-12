// testExe.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <Eigen/Dense>
#include <Eigen/Geometry>
#include <iostream>



int main()
{
	std::cout << "start\n";

	Eigen::MatrixXf tr1Glob{5,3};

	tr1Glob.col(0) <<
		Eigen::VectorXf::LinSpaced(2, 1, 1),
		1,
		Eigen::VectorXf::LinSpaced(2, 1, 1);
	tr1Glob.col(1) <<
		Eigen::VectorXf::LinSpaced(2, 2, 2),
		2,
		Eigen::VectorXf::LinSpaced(2, 2, 2);
	tr1Glob.col(2) <<
		Eigen::VectorXf::LinSpaced(2, 3, 3),
		3,
		Eigen::VectorXf::LinSpaced(2, 3, 3);

	std::cout << tr1Glob << "\n";


	for (int i = 0; i < 0; i++)
	{
		std::cout << "print\n";
	}

	std::cout << "end\n";


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
