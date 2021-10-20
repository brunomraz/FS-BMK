// testExe.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <Eigen/Dense>
#include <Eigen/Geometry>
#include <iostream>

float* test_f()
{
	float returnFloat[] ={ 5.0f,6.0f };
	return returnFloat;
}


class Test
{
	// members
private:
	Eigen::MatrixXf lca3Glob;// (vertIncr * 2 + 1, 3);

public:
	Test(int size):lca3Glob(size, size)
	{
		
		
	}

	void printTest()
	{
		lca3Glob <<
			1, 2, 3,
			4, 5, 6,
			7, 8, 9;
		std::cout << lca3Glob;
	}

};

int main()
{
	Test newTest{3};
	float outputFloat[5];

	*outputFloat = test_f();

	newTest.printTest();


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
