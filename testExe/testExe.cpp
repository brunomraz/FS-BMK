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
//ArrayXf v = ArrayXf::LinSpaced(11, 0.f, 10.f);
//// vc is the corresponding C array. Here's how you can use it yourself:
//float* vc = v.data();
//cout << vc[3] << endl;  // 3.0
//// Or you can give it to some C api call that takes a C array:
//some_c_api_call(vc, v.size());
//// Be careful not to use this pointer after v goes out of scope! If
//// you still need the data after this point, you must copy vc. This can
//// be done using in the usual C manner, or with Eigen's Map<> class.

class Test
{
	// members
private:
	Eigen::MatrixXf lca3Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf uca3Glob;// (vertIncr * 2 + 1, 3);

public:
	Test(int size):lca3Glob(size, size), uca3Glob(size, size)
	{
		
		
	}

	void printTest()
	{
		lca3Glob <<
			1, 2, 3,
			4, 5, 6,
			7, 8, 9;

		uca3Glob <<
			10, 20, 30,
			40, 50, 60,
			70, 80, 90;


		float outVec[6];

		outVec[0] = lca3Glob.row(0)(0),		outVec[1] = lca3Glob.row(0)(1),		outVec[2] = lca3Glob.row(0)(2);
		std::cout << outVec[0];
		std::cout << outVec[1];
		std::cout << outVec[2];
		std::cout << "\n";
		std::cout << lca3Glob.row(0).data();
	}

};

int main()
{
	Test newTest{3};
	float outputFloat[5];


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
