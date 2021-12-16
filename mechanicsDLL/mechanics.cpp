#include "pch.h"
#include "mechanics.h"
#include <iostream>
#include <Eigen/Dense>
#include <Eigen/Geometry>
#include <vector>
#include <sstream>
#include <math.h>

template <typename T> int sgn(T val) {
	return (T(0) < val) - (val < T(0));
}

class Suspension
{

	// variables
private:
	// suffix ref stands for ref, so reference/initial coordinates can be easily
	// differentiated from coordinates of the same points during wheel movement
	// hardpoints
	// LCA
	Eigen::Vector3f lca1ref;
	Eigen::Vector3f lca2ref;
	Eigen::Vector3f lca3ref;

	Eigen::Vector3f uca1ref;
	Eigen::Vector3f uca2ref;
	Eigen::Vector3f uca3ref;

	Eigen::Vector3f tr1ref;
	Eigen::Vector3f tr2ref;

	Eigen::Vector3f wcnref;
	Eigen::Vector3f spnref;




	float wRadius;     // wheel radius	
	float wVert;       // wheel vertical movement	
	float wSteer;      // wheel steering movement	
	int vertIncr;      // number of increments between reference position and upmost and downmost
	int steerIncr;     // number of increments between reference position and leftmost and rightmost	
	float precision;   // precision- at what value has the iterator converged, in percentage- 0...1

	float wheelbase;
	float cogHeight;

	float driveBias;
	float brakeBias;

	int suspPos;      // front or rear suspension 0 for front, 1 for rear	
	int drivePos;     // outboard or inboard drive 0 for outboard, 1 for inboard	
	int brakePos;     // outboard or inboard brakes 0 for outboard, 1 for inboard


	// derived values

	Eigen::Vector3f lca12;
	Eigen::Vector3f uca12;
	Eigen::MatrixXf lca3Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf uca3Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf tr1Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf tr2Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf wcnGlob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf spnGlob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf cpGlob;// (vertIncr * 2 + 1, 3);


public:


	Suspension(
		float* hps,
		float wRadiusin,
		float wheelbasein, float cogHeightin, float driveBiasin,
		float brakeBiasin, int suspPosin, int drivePosin, int brakePosin,
		float wVertin, float wSteerin,
		int vertIncrin, int steerIncrin, float precisionin) :
		lca3Glob(vertIncrin * 2 + 1, 3), uca3Glob(vertIncrin * 2 + 1, 3), 
		tr1Glob(steerIncrin * 2 + 1, 3), 
		tr2Glob((vertIncrin * 2 + 1) * (steerIncrin * 2 + 1), 3), wcnGlob((vertIncrin * 2 + 1) * (steerIncrin * 2 + 1), 3), 
		spnGlob((vertIncrin * 2 + 1) * (steerIncrin * 2 + 1), 3), cpGlob((vertIncrin * 2 + 1) * (steerIncrin * 2 + 1), 3)
	{
		lca1ref << hps[0], hps[1], hps[2];
		lca2ref << hps[3], hps[4], hps[5];
		lca3ref << hps[6], hps[7], hps[8];

		uca1ref << hps[9], hps[10], hps[11];
		uca2ref << hps[12], hps[13], hps[14];
		uca3ref << hps[15], hps[16], hps[17];

		tr1ref << hps[18], hps[19], hps[20];
		tr2ref << hps[21], hps[22], hps[23];

		wcnref << hps[24], hps[25], hps[26];
		spnref << hps[27], hps[28], hps[29];

		wRadius = wRadiusin;

		wheelbase = wheelbasein;
		cogHeight = cogHeightin;
		driveBias = driveBiasin;

		brakeBias = brakeBiasin;

		suspPos = suspPosin;
		drivePos = drivePosin;
		brakePos = brakePosin;

		wVert = wVertin;
		wSteer = wSteerin;
		vertIncr = vertIncrin;
		steerIncr = steerIncrin;
		precision = precisionin;

	}


	// FUNCTIONS
private:

	// place as inputs variables rLCA, rUCA, uca12, lca12, etc. so those can be only temporary
	// values and not use up stack memory

	void CalculateConstants(
		Eigen::Matrix3f& _rotLCA, Eigen::Matrix3f& _rotUCA,
		float& _rLCA, float& _rUCA, float& _rCA,
		Eigen::ArrayXf& _zLocLca,
		float& _rST, float& _t_param, float& _rTR,
		Eigen::Vector3f& _wcnlocTRk, Eigen::Vector3f& _spnlocTRk)
	{

		// create line connecting lca1 and lca2
		Eigen::ParametrizedLine<float, 3> lca1lca2 = Eigen::ParametrizedLine<float, 3>::Through(lca1ref, lca2ref);

		// local LCA plane for determining max z value of wheel parameters
		Eigen::Vector4f abcd;
		Eigen::Vector3f _tr2prref;

		// z local maximum value
		float zLocHi;
		// z local minimum value
		float zLocLo;

		lca12 = lca1lca2.projection(lca3ref);
		_rLCA = lca1lca2.distance(lca3ref);

		// create rotation matrix for LCA cs
		_rotLCA <<
			(lca1ref - lca2ref).normalized(),
			(lca12 - lca3ref).normalized(),
			((lca1ref - lca2ref).cross(lca12 - lca3ref)).normalized();

		// calculate parameters for plane in LCA cs ax+by+cz+d=0
		abcd <<
			_rotLCA.row(2)(0),
			_rotLCA.row(2)(1),
			_rotLCA.row(2)(2),
			-_rotLCA.row(2) * (_rotLCA.transpose() * Eigen::Vector3f{ -lca12(0),-lca12(1),lca3ref(2) - wVert - lca12(2) });
		// calculates z value for upmost movement of wheel for intersection of plane and circle in LCA
		zLocHi =
			(-abcd(2) * abcd(3) +
				abcd(1) * sqrt(abcd(2) * abcd(2) * _rLCA * _rLCA +
					abcd(1) * abcd(1) * _rLCA * _rLCA -
					abcd(3) * abcd(3))) /
			(abcd(1) * abcd(1) + abcd(2) * abcd(2));

		// reuses previous parameters for LCA plane, only 4th parameter is changed
		abcd(3) =
			-_rotLCA.row(2) *
			_rotLCA.transpose() *
			Eigen::Vector3f{ -lca12(0), -lca12(1), lca3ref(2) + wVert - lca12(2) };

		// calculates z value for downmost movement of wheel for intersection of plane and circle in LCA
		zLocLo =
			(-abcd(2) * abcd(3) +
				abcd(1) * sqrt(abcd(2) * abcd(2) * _rLCA * _rLCA +
					abcd(1) * abcd(1) * _rLCA * _rLCA -
					abcd(3) * abcd(3))) /
			(abcd(1) * abcd(1) + abcd(2) * abcd(2));

		// wheel travel from rebound to bump
		_zLocLca <<
			Eigen::VectorXf::LinSpaced(vertIncr, zLocLo, zLocLo / vertIncr),
			0,
			Eigen::VectorXf::LinSpaced(vertIncr, zLocHi / vertIncr, zLocHi);


		// create line connecting uca1 and uca2
		Eigen::ParametrizedLine<float, 3> uca1uca2 = Eigen::ParametrizedLine<float, 3>::Through(uca1ref, uca2ref);

		uca12 = uca1uca2.projection(uca3ref);
		_rUCA = uca1uca2.distance(uca3ref);

		_rCA = (uca3ref - lca3ref).norm();

		// create rotation matrix for UCA cs
		_rotUCA <<
			(uca1ref - uca2ref).normalized(),
			(uca12 - uca3ref).normalized(),
			((uca1ref - uca2ref).cross(uca12 - uca3ref)).normalized();


		// calculate TR2 projection point and rTR
		Eigen::ParametrizedLine<float, 3> lca3uca3 = Eigen::ParametrizedLine<float, 3>::Through(lca3ref, uca3ref);

		_tr2prref = lca3uca3.projection(tr2ref);

		_rST = (tr2ref - _tr2prref).norm();

		_rTR = (tr2ref - tr1ref).norm();

		_t_param = (_tr2prref(2) - lca3ref(2)) / (uca3ref(2) - lca3ref(2));



		Eigen::Matrix3f _rotTRk; // TR rotation matrix defined by TR2ref

		Eigen::Vector3f _xCol{ _tr2prref - tr2ref };
		Eigen::Vector3f _zCol{ lca3ref - uca3ref };
		Eigen::Vector3f _yCol{ _zCol.cross(_xCol) };


		_rotTRk.col(0) << _xCol / _xCol.norm();
		_rotTRk.col(1) << _yCol / _yCol.norm();
		_rotTRk.col(2) << _zCol / _zCol.norm();

		_wcnlocTRk << _rotTRk.transpose() * (wcnref - _tr2prref);
		_spnlocTRk << _rotTRk.transpose() * (spnref - _tr2prref);
	}

public:

	void CalculateMovement()
	{
		Eigen::Matrix3f rotLCA;
		Eigen::Matrix3f rotUCA;
		Eigen::MatrixXf tr2prGlob;
		float rLCA;
		float rUCA;
		float rCA;   // distance between LCA3 and UCA3
		float rST;   // distance between TR2 and UCA3LCA3 axis
		float rTR;   // distance between TR2 and TR1 axis
		float t_param;   // parameter to determine position of TR2pr on uca3lca3 line
		// coordinates of wcn and spn in TR cs defined by tr2ref
		Eigen::Vector3f wcnlocTRk;
		Eigen::Vector3f spnlocTRk;

		Eigen::ArrayXf zLCA3LocLCA(vertIncr * 2 + 1);

		Suspension::CalculateConstants(rotLCA, rotUCA, rLCA, rUCA, rCA, zLCA3LocLCA, rST, t_param, rTR, wcnlocTRk, spnlocTRk);



		Eigen::MatrixXf lca3LocLCA(vertIncr * 2 + 1, 3);

		// populating positions of local LCA3 in a matrix
		lca3LocLCA.col(0) << Eigen::VectorXf::Zero(vertIncr * 2 + 1);
		lca3LocLCA.col(1) << -(rLCA * rLCA - zLCA3LocLCA * zLCA3LocLCA).sqrt();
		lca3LocLCA.col(2) << zLCA3LocLCA;




		// global positions of LCA3 for whole wheel movement
		lca3Glob = (lca3LocLCA * rotLCA.transpose()).array().rowwise() + lca12.array().transpose();


		// global position of UCA3 for whole wheel movement
		Eigen::MatrixXf uca3LocUCA(vertIncr * 2 + 1, 3);
		Eigen::MatrixXf lca3LocUCA(vertIncr * 2 + 1, 3);

		lca3LocUCA = lca3Glob.rowwise() - uca12.transpose();
		lca3LocUCA = lca3LocUCA * rotUCA;


		// temporary values for calculating UCA3 in UCA cs, correspond to chunks of expression in word
		Eigen::ArrayXf temp1UCA3 =
			-rCA * rCA + rUCA * rUCA +
			lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() +
			lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() +
			lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array();

		Eigen::ArrayXf temp2UCA3 =
			2 * (lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() +
				lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array());

		Eigen::ArrayXf temp3UCA3 =
			-rCA * rCA * rCA * rCA + 2 * rCA * rCA * rUCA * rUCA +
			2 * rCA * rCA * lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() +
			2 * rCA * rCA * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() +
			2 * rCA * rCA * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array();

		Eigen::ArrayXf temp4UCA3 =
			-rUCA * rUCA * rUCA * rUCA -
			2 * rUCA * rUCA * lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() +
			2 * rUCA * rUCA * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() +
			2 * rUCA * rUCA * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array();

		Eigen::ArrayXf temp5UCA3 =
			-lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() -
			2 * lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() -
			2 * lca3LocUCA.col(0).array() * lca3LocUCA.col(0).array() * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array();

		Eigen::ArrayXf temp6UCA3 =
			-2 * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array() -
			lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() -
			lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array();

		Eigen::ArrayXf temp7UCA3 = (temp3UCA3 + temp4UCA3 + temp5UCA3 + temp6UCA3).sqrt();

		uca3LocUCA.col(0) << Eigen::VectorXf::Zero(vertIncr * 2 + 1);
		uca3LocUCA.col(1) << (lca3LocUCA.col(1).array() * temp1UCA3 - lca3LocUCA.col(2).array() * temp7UCA3) / temp2UCA3;
		uca3LocUCA.col(2) << (lca3LocUCA.col(2).array() * temp1UCA3 + lca3LocUCA.col(1).array() * temp7UCA3) / temp2UCA3;


		uca3Glob = (uca3LocUCA * rotUCA.transpose()).array().rowwise() + uca12.array().transpose();

		

		// calculating TR2 positions

		tr1Glob.col(0) <<
			Eigen::VectorXf::LinSpaced(2 * steerIncr + 1, tr1ref(0), tr1ref(0));
		tr1Glob.col(1) <<
			Eigen::VectorXf::LinSpaced(steerIncr, tr1ref(1) - wSteer, tr1ref(1) - wSteer / steerIncr),
			tr1ref(1),
			Eigen::VectorXf::LinSpaced(steerIncr, tr1ref(1) + wSteer / steerIncr, tr1ref(1) + wSteer);
		tr1Glob.col(2) <<
			Eigen::VectorXf::LinSpaced(2 * steerIncr + 1, tr1ref(2), tr1ref(2));

		tr2prGlob = (uca3Glob - lca3Glob) * t_param + lca3Glob;


		for (int i = 0; i < vertIncr * 2 + 1; i++)
		{
			Eigen::Matrix3f rotTR;
			Eigen::Vector3f tr2locTR;

			Eigen::Vector3f xCol{ 30, 0, -30 * (uca3Glob(i, 0) - lca3Glob(i, 0)) / (uca3Glob(i, 2) - lca3Glob(i, 2)) };
			Eigen::Vector3f zCol{ (lca3Glob.row(i) - uca3Glob.row(i)).transpose() };
			Eigen::Vector3f yCol{ zCol.cross(xCol) };


			rotTR.col(0) << xCol / xCol.norm();
			rotTR.col(1) << yCol / yCol.norm();
			rotTR.col(2) << zCol / zCol.norm();
			for (int j = 0; j < steerIncr * 2 + 1; j++)
			{
				// calculate local position of TR1
				Eigen::Vector3f tr1locTR;

				tr1locTR = -tr2prGlob.row(i) + tr1Glob.row(j);
				tr1locTR = rotTR.transpose() * tr1locTR;


				// calculate local position of TR2
				float temp1TR2 =
					rST * rST - rTR * rTR +
					tr1locTR(0) * tr1locTR(0) +
					tr1locTR(1) * tr1locTR(1) +
					tr1locTR(2) * tr1locTR(2);

				float temp2TR2 = 2 * (tr1locTR(0) * tr1locTR(0) + tr1locTR(1) * tr1locTR(1));

				float temp3TR2 =
					-rST * rST * rST * rST + 2 * rTR * rTR * rST * rST +
					2 * rST * rST * tr1locTR(0) * tr1locTR(0) +
					2 * rST * rST * tr1locTR(1) * tr1locTR(1) -
					2 * rST * rST * tr1locTR(2) * tr1locTR(2);

				float temp4TR2 =
					-rTR * rTR * rTR * rTR +
					2 * rTR * rTR * tr1locTR(0) * tr1locTR(0) +
					2 * rTR * rTR * tr1locTR(1) * tr1locTR(1) +
					2 * rTR * rTR * tr1locTR(2) * tr1locTR(2);

				float temp5TR2 =
					-tr1locTR(0) * tr1locTR(0) * tr1locTR(0) * tr1locTR(0) -
					2 * tr1locTR(0) * tr1locTR(0) * tr1locTR(1) * tr1locTR(1) -
					2 * tr1locTR(0) * tr1locTR(0) * tr1locTR(2) * tr1locTR(2);

				float temp6TR2 =
					-2 * tr1locTR(1) * tr1locTR(1) * tr1locTR(2) * tr1locTR(2) -
					tr1locTR(1) * tr1locTR(1) * tr1locTR(1) * tr1locTR(1) -
					tr1locTR(2) * tr1locTR(2) * tr1locTR(2) * tr1locTR(2);

				float temp7TR2 = sgn(wcnref[0] - tr2ref[0]) * std::sqrt(temp3TR2 + temp4TR2 + temp5TR2 + temp6TR2);



				tr2locTR(0) = (tr1locTR(0) * temp1TR2 - tr1locTR(1) * temp7TR2) / temp2TR2;
				tr2locTR(1) = (tr1locTR(1) * temp1TR2 + tr1locTR(0) * temp7TR2) / temp2TR2;
				tr2locTR(2) = 0;

				tr2Glob.row(i * (2 * steerIncr + 1) + j) << (rotTR * tr2locTR).transpose() + tr2prGlob.row(i);



				// calculating WCN and SPN 
				Eigen::Matrix3f _rotTRk; // TR rotation matrix defined by TR2ref

				Eigen::Vector3f _xCol{ tr2prGlob.row(i) - tr2Glob.row(i * (2 * steerIncr + 1) + j) };
				Eigen::Vector3f _zCol{ lca3Glob.row(i) - uca3Glob.row(i) };
				Eigen::Vector3f _yCol{ _zCol.cross(_xCol) };


				_rotTRk.col(0) << _xCol / _xCol.norm();
				_rotTRk.col(1) << _yCol / _yCol.norm();
				_rotTRk.col(2) << _zCol / _zCol.norm();

				wcnGlob.row(i * (2 * steerIncr + 1) + j) << (_rotTRk * wcnlocTRk).transpose() + tr2prGlob.row(i);
				spnGlob.row(i * (2 * steerIncr + 1) + j) << (_rotTRk * spnlocTRk).transpose() + tr2prGlob.row(i);


				// CP calculation
				float temp1cp{ -20 }; // this is actually vector 0,0,-20
				Eigen::MatrixXf temp2cp((vertIncr * 2 + 1) * (steerIncr * 2 + 1), 3);
				Eigen::MatrixXf temp3cp((vertIncr * 2 + 1) * (steerIncr * 2 + 1), 3);

				temp2cp << spnGlob - wcnGlob;

				temp3cp.col(0) << -temp2cp.col(0).array() * temp2cp.col(2).array() * temp1cp;
				temp3cp.col(1) << -temp2cp.col(1).array() * temp2cp.col(2).array() * temp1cp;
				temp3cp.col(2) <<
					temp2cp.col(1).array() * temp2cp.col(1).array() * temp1cp +
					temp2cp.col(0).array() * temp2cp.col(0).array() * temp1cp;

				temp3cp.rowwise().normalize();


				cpGlob << -temp3cp * wRadius + wcnGlob;
			}
		}
	}

	void LogToConsole()
	{

		std::cout << "lca3\n " << lca3Glob << "\n";
		std::cout << "\n";
		std::cout << "uca3\n " << uca3Glob << "\n";
		std::cout << "\n";

		std::cout << "tr1\n " << tr1Glob << "\n";
		std::cout << "\n";

		std::cout << "tr2\n " << tr2Glob << "\n";
		std::cout << "\n";

		std::cout << "wcn\n " << wcnGlob << "\n";
		std::cout << "\n";

		std::cout << "spn\n " << spnGlob << "\n";
		std::cout << "\n";

		std::cout << "cp\n " << cpGlob << "\n";
		std::cout << "\n";

	}

	float ObjFuncModule(float peakWidth, float flatness, float variable, float target)
	{
		return (float)exp(-1 / peakWidth * pow(abs(variable - target), flatness));
	}

	float GetObjFuncScore(float* peakWidth, float* flatness, float* variables, float* targets, float* weightFactors)
	{

		float objFuncScore = 1.0f;

		for (int i = 0; i < 21; i++)
			objFuncScore -= weightFactors[i] * ObjFuncModule(peakWidth[i], flatness[i], variables[i], targets[i]);

		return objFuncScore;
	}

	float GetCamberAngle(int vertPos, int steerPos)
	{
		float camberAngle;
		int L = vertPos * (2 * steerIncr + 1) + steerPos;
		int R = (2 * vertIncr - vertPos) * (2 * steerIncr + 1) + 2 * steerIncr - steerPos;


		Eigen::Vector3f wheelAxis{
			-wcnGlob.row(L)(0) + cpGlob.row(L)(0),
			-wcnGlob.row(L)(1) + cpGlob.row(L)(1),
			-wcnGlob.row(L)(2) + cpGlob.row(L)(2)
		};


		Eigen::Vector3f groundNormal{
			0,
			-cpGlob.row(R)(2) + cpGlob.row(L)(2),
			-cpGlob.row(R)(1) - cpGlob.row(L)(1)
		};
		std::cout << "vertPos" << vertPos << "\n";
		std::cout << "steerPos" << steerPos << "\n";
		std::cout << "R" << R << "\n";
		std::cout << "L" << L << "\n";

		// calculate plane parallel to ground going through SPN point with respect to which camber is measured
		float temp1_wcnpr =
			spnGlob.row(L)(1) * groundNormal(1)
			+ spnGlob.row(L)(2) * groundNormal(2)
			- wcnGlob.row(L)(1) * groundNormal(1)
			- wcnGlob.row(L)(2) * groundNormal(2);
		float temp2_wcnpr =
			groundNormal(1) * groundNormal(1) +
			groundNormal(2) * groundNormal(2);

		Eigen::Vector3f wcnpr{
			wcnGlob.row(L)(0),
			wcnGlob.row(L)(1) + groundNormal(1) * temp1_wcnpr / temp2_wcnpr,
			wcnGlob.row(L)(2) + groundNormal(2) * temp1_wcnpr / temp2_wcnpr
		};

		float camber =
			(wcnpr - (Eigen::Vector3f)wcnGlob.row(L)).norm() /
			((Eigen::Vector3f)spnGlob.row(L) -
				(Eigen::Vector3f)wcnGlob.row(L)).norm();

		// tests if camber is negative, if it is it returns negative angle
		if ((wcnpr - (Eigen::Vector3f)wcnGlob.row(L))(2) > 0)
		{
			camberAngle = -asin(camber) * 180 / 3.14159f;
			return camberAngle;
		}

		// if camber is not negative, returns positive angle
		else
		{
			camberAngle = asin(camber) * 180 / 3.14159f;
			return camberAngle;
		}
	}

	float GetToeAngle(int vertPos, int steerPos)
	{
		int position = vertPos * (2 * steerIncr + 1) + steerPos;
		// positive toe angle for toe in and negative for toe out
		Eigen::Vector3f wheelAxis = wcnGlob.row(position) - spnGlob.row(position);
		Eigen::Vector3f refAxis{
			0,
			wcnGlob.row(position)(1) - spnGlob.row(position)(1),
			wcnGlob.row(position)(2) - spnGlob.row(position)(2)
		};

		if (wcnGlob.row(position)(0) < spnGlob.row(position)(0)) // toe out case
			return -acos(refAxis.norm() / wheelAxis.norm()) * 180 / 3.14159f;

		else // toe in case
			return acos(refAxis.norm() / wheelAxis.norm()) * 180 / 3.14159f;
	}

	float GetCasterAngle(int vertPos)
	{
		int position = vertPos;
		float casterAngle;
		float caster =
			atan2f(
				(lca3Glob.row(position)(0) - uca3Glob.row(position)(0))
				, (-uca3Glob.row(position)(2) + lca3Glob.row(position)(2)));

		casterAngle = caster * 180 / 3.14159f;
		return casterAngle;
	}

	float GetRollCentreHeight(int vertPos, int steerPos)
	{
		float rollCentreHeight;

		int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		int Rv = 2 * vertIncr - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;									  // Left wheel position including vertical and steering movement
		int Rvs = (2 * vertIncr - vertPos) * (2 * steerIncr + 1) + 2 * steerIncr - steerPos;  // Right wheel position including vertical and steering movement


		float slopePrecision{ 0.001f }; // if difference between slopes is less than this value, than they are considered parallel

		Eigen::Vector3f lca3L{ lca3Glob.row(Lv) };
		Eigen::Vector3f uca3L{ uca3Glob.row(Lv) };
		Eigen::Vector3f cpL{ cpGlob.row(Lvs) };

		Eigen::Vector3f lca1R{ lca1ref(0), -lca1ref(1), lca1ref(2) };
		Eigen::Vector3f lca2R{ lca2ref(0), -lca2ref(1), lca2ref(2) };
		Eigen::Vector3f lca3R{ lca3Glob.row(Rv)(0), -lca3Glob.row(Rv)(1), lca3Glob.row(Rv)(2) };

		Eigen::Vector3f uca1R{ uca1ref(0), -uca1ref(1), uca1ref(2) };
		Eigen::Vector3f uca2R{ uca2ref(0), -uca2ref(1), uca2ref(2) };
		Eigen::Vector3f uca3R{ uca3Glob.row(Rv)(0), -uca3Glob.row(Rv)(1), uca3Glob.row(Rv)(2) };
		Eigen::Vector3f cpR{ cpGlob.row(Rvs)(0), -cpGlob.row(Rvs)(1), cpGlob.row(Rvs)(2) };

		float aLCAL;
		float aUCAL;
		float aLCAR;
		float aUCAR;

		float bLCAL;
		float bUCAL;
		float bLCAR;
		float bUCAR;

		// LCA and UCA plane intersection with plane parallel to YZ plane with x coord CPR/2+CPL/2
		// lambda function that defines first point of intersection line
		auto intersectionLineCalc = [cpL, cpR](float& aCa, float& bCa, const Eigen::Vector3f& ca1, const Eigen::Vector3f& ca2, const Eigen::Vector3f& ca3)
		{
			// plane coefficients Ax + By + Cz + D = 0, plane defined by control arm
			float A = (-ca1[1] + ca2[1]) * (-ca1[2] + ca3[2]) - (-ca1[1] + ca3[1]) * (-ca1[2] + ca2[2]);
			float B = -(-ca1[0] + ca2[0]) * (-ca1[2] + ca3[2]) + (-ca1[0] + ca3[0]) * (-ca1[2] + ca2[2]);
			float C = (-ca1[0] + ca2[0]) * (-ca1[1] + ca3[1]) - (-ca1[0] + ca3[0]) * (-ca1[1] + ca2[1]);
			float D = -ca1[0] * A - ca1[1] * B - ca1[2] * C;

			// intersection plane defined  as x + D2 = 0
			float D2 = -(cpL[0] + cpR[0]) / 2;

			aCa = -B / C;
			bCa = (A * D2 - D) / C;
		};

		intersectionLineCalc(aLCAL, bLCAL, lca1ref, lca2ref, lca3L);
		intersectionLineCalc(aUCAL, bUCAL, uca1ref, uca2ref, uca3L);
		intersectionLineCalc(aLCAR, bLCAR, lca1R, lca2R, lca3R);
		intersectionLineCalc(aUCAR, bUCAR, uca1R, uca2R, uca3R);

		float aICL;
		float aICR;
		float bICL;
		float bICR;

		// CALCULATE LEFT SIDE
		// case if LEFT LCA and UCA are parallel
		if (abs(aLCAL - aUCAL) / abs(aLCAL) < precision)
		{
			aICL = aLCAL;
			bICL = cpL(2) - aLCAL * cpL(1);
		}
		// case if LEFT LCA and UCA are NOT parallel
		else
		{
			float ICLz = (aLCAL * bUCAL - aUCAL * bLCAL) / (aLCAL - aUCAL);
			float ICLy = (-bLCAL + bUCAL) / (aLCAL - aUCAL);

			aICL = (ICLz - cpL(2)) / (ICLy - cpL(1));
			bICL = -cpL(1) * aICL + cpL(2);
		}

		// CALCULATE RIGHT SIDE
		// case if RIGHT LCA and UCA are parallel
		if (abs(aLCAR - aUCAR) / abs(aLCAR) < precision)
		{
			aICR = aLCAR;
			bICR = cpR(2) - aLCAR * cpR(1);
		}
		// case if RIGHT LCA and UCA are NOT parallel
		else
		{
			float ICRz = (aLCAR * bUCAR - aUCAR * bLCAR) / (aLCAR - aUCAR);
			float ICRy = (-bLCAR + bUCAR) / (aLCAR - aUCAR);

			aICR = (ICRz - cpR(2)) / (ICRy - cpR(1));
			bICR = -cpR(1) * aICR + cpR(2);
		}

		// instantenous centre lines are parallel
		if (abs(aICR - aICL) / abs(aICR) < precision)
		{
			rollCentreHeight = 0;
			return rollCentreHeight;
		}

		// instantenous centre lines are NOT parallel
		else
		{
			float RCy = (bICL - bICR) / (aICR - aICL);
			float RCz = aICL * RCy + bICL;

			rollCentreHeight =
				((cpR(1) - cpL(1)) * (cpL(2) - RCz) -
					(cpL(1) - RCy) * (cpR(2) - cpL(2))) /
				sqrt(pow((cpR(2) - cpL(2)), 2) + pow((cpR(1) - cpL(1)), 2));
			return rollCentreHeight;
		}
	}

	float GetCasterTrail(int vertPos, int steerPos)
	{
		float casterTrail;


		int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		int Rv = 2 * vertIncr - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;									  // Left wheel position including vertical and steering movement
		int Rvs = (2 * vertIncr - vertPos) * (2 * steerIncr + 1) + 2 * steerIncr - steerPos;  // Right wheel position including vertical and steering movement


		//int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		//int Rv = cpGlob.rows() - 1 - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		//int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;							// Left wheel position including vertical and steering movement
		//int Rvs = (cpGlob.rows() - 1 - vertPos) * (2 * steerIncr + 1) + steerPos;		// Right wheel position including vertical and steering movement

		Eigen::Vector3f cpL{ cpGlob.row(Lvs) };
		Eigen::Vector3f cpR{ cpGlob.row(Rvs) };
		Eigen::Vector3f wcn{ wcnGlob.row(Lvs) };
		Eigen::Vector3f spn{ spnGlob.row(Lvs) };
		Eigen::Vector3f lca3L{ lca3Glob.row(Lv) };
		Eigen::Vector3f uca3L{ uca3Glob.row(Lv) };

		Eigen::Vector3f grndNormal{
			0,
			-cpR(2) + cpL(2),
			-cpR(1) - cpL(1)
		};

		float wcnpr_temp1 =
			grndNormal(0) * cpL(0) - grndNormal(0) * wcn(0) +
			grndNormal(1) * cpL(1) - grndNormal(1) * wcn(1) +
			grndNormal(2) * cpL(2) - grndNormal(2) * wcn(2);

		float wcnpr_temp2 =
			grndNormal(0) * grndNormal(0) +
			grndNormal(1) * grndNormal(1) +
			grndNormal(2) * grndNormal(2);

		Eigen::Vector3f wcnpr{
			grndNormal(0) * wcnpr_temp1 / wcnpr_temp2 + wcn(0),
			grndNormal(1) * wcnpr_temp1 / wcnpr_temp2 + wcn(1),
			grndNormal(2) * wcnpr_temp1 / wcnpr_temp2 + wcn(2)
		};

		float spnpr_temp1 =
			grndNormal(0) * cpL(0) - grndNormal(0) * wcn(0) +
			grndNormal(1) * cpL(1) - grndNormal(1) * wcn(1) +
			grndNormal(2) * cpL(2) - grndNormal(2) * wcn(2);

		float spnpr_temp2 =
			grndNormal(0) * grndNormal(0) +
			grndNormal(1) * grndNormal(1) +
			grndNormal(2) * grndNormal(2);

		Eigen::Vector3f spnpr{
			grndNormal(0) * spnpr_temp1 / spnpr_temp2 + spn(0),
			grndNormal(1) * spnpr_temp1 / spnpr_temp2 + spn(1),
			grndNormal(2) * spnpr_temp1 / spnpr_temp2 + spn(2)
		};

		float l3u3intrs_temp1 =
			-grndNormal(0) * cpL(0) + grndNormal(0) * lca3L(0)
			- grndNormal(1) * cpL(1) + grndNormal(1) * lca3L(1)
			- grndNormal(2) * cpL(2) + grndNormal(2) * lca3L(2);

		float l3u3intrs_temp2 =
			grndNormal(0) * lca3L(0) - grndNormal(0) * uca3L(0) +
			grndNormal(1) * lca3L(1) - grndNormal(1) * uca3L(1) +
			grndNormal(2) * lca3L(2) - grndNormal(2) * uca3L(2);

		Eigen::Vector3f l3u3intrs{
			lca3L(0) - (lca3L(0) - uca3L(0)) * l3u3intrs_temp1 / l3u3intrs_temp2,
			lca3L(1) - (lca3L(1) - uca3L(1)) * l3u3intrs_temp1 / l3u3intrs_temp2,
			lca3L(2) - (lca3L(2) - uca3L(2)) * l3u3intrs_temp1 / l3u3intrs_temp2
		};

		float caster_trail =
			(l3u3intrs - spnpr).cross(l3u3intrs - wcnpr).norm() / (wcnpr - spnpr).norm();


		// positive caster trail
		if ((l3u3intrs - spnpr).cross(l3u3intrs - wcnpr)(2) > 0)
		{
			casterTrail = caster_trail;

			return casterTrail;
		}

		// negative caster trail
		else
		{
			casterTrail = -caster_trail;
			return casterTrail;
		}
	}

	float GetScrubRadius(int vertPos, int steerPos)
	{
		float scrubRadius;

		int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		int Rv = 2 * vertIncr - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;									  // Left wheel position including vertical and steering movement
		int Rvs = (2 * vertIncr - vertPos) * (2 * steerIncr + 1) + 2 * steerIncr - steerPos;  // Right wheel position including vertical and steering movement

		//int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		//int Rv = cpGlob.rows() - 1 - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		//int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;							// Left wheel position including vertical and steering movement
		//int Rvs = (cpGlob.rows() - 1 - vertPos) * (2 * steerIncr + 1) + steerPos;		// Right wheel position including vertical and steering movement

		Eigen::Vector3f cpL{ cpGlob.row(Lvs) };
		Eigen::Vector3f cpR{ cpGlob.row(Rvs) };
		Eigen::Vector3f wcn{ wcnGlob.row(Lvs) };
		Eigen::Vector3f spn{ spnGlob.row(Lvs) };
		Eigen::Vector3f lca3L{ lca3Glob.row(Lv) };
		Eigen::Vector3f uca3L{ uca3Glob.row(Lv) };

		Eigen::Vector3f grndNormal{
			0,
			-cpR(2) + cpL(2),
			-cpR(1) - cpL(1)
		};

		float wcnpr_temp1 =
			grndNormal(0) * cpL(0) - grndNormal(0) * wcn(0) +
			grndNormal(1) * cpL(1) - grndNormal(1) * wcn(1) +
			grndNormal(2) * cpL(2) - grndNormal(2) * wcn(2);

		float wcnpr_temp2 =
			grndNormal(0) * grndNormal(0) +
			grndNormal(1) * grndNormal(1) +
			grndNormal(2) * grndNormal(2);

		Eigen::Vector3f wcnpr{
			grndNormal(0) * wcnpr_temp1 / wcnpr_temp2 + wcn(0),
			grndNormal(1) * wcnpr_temp1 / wcnpr_temp2 + wcn(1),
			grndNormal(2) * wcnpr_temp1 / wcnpr_temp2 + wcn(2)
		};

		float spnpr_temp1 =
			grndNormal(0) * cpL(0) - grndNormal(0) * wcn(0) +
			grndNormal(1) * cpL(1) - grndNormal(1) * wcn(1) +
			grndNormal(2) * cpL(2) - grndNormal(2) * wcn(2);

		float spnpr_temp2 =
			grndNormal(0) * grndNormal(0) +
			grndNormal(1) * grndNormal(1) +
			grndNormal(2) * grndNormal(2);

		Eigen::Vector3f spnpr{
			grndNormal(0) * spnpr_temp1 / spnpr_temp2 + spn(0),
			grndNormal(1) * spnpr_temp1 / spnpr_temp2 + spn(1),
			grndNormal(2) * spnpr_temp1 / spnpr_temp2 + spn(2)
		};

		float l3u3intrs_temp1 =
			-grndNormal(0) * cpL(0) + grndNormal(0) * lca3L(0)
			- grndNormal(1) * cpL(1) + grndNormal(1) * lca3L(1)
			- grndNormal(2) * cpL(2) + grndNormal(2) * lca3L(2);

		float l3u3intrs_temp2 =
			grndNormal(0) * lca3L(0) - grndNormal(0) * uca3L(0) +
			grndNormal(1) * lca3L(1) - grndNormal(1) * uca3L(1) +
			grndNormal(2) * lca3L(2) - grndNormal(2) * uca3L(2);

		Eigen::Vector3f l3u3intrs{
			lca3L(0) - (lca3L(0) - uca3L(0)) * l3u3intrs_temp1 / l3u3intrs_temp2,
			lca3L(1) - (lca3L(1) - uca3L(1)) * l3u3intrs_temp1 / l3u3intrs_temp2,
			lca3L(2) - (lca3L(2) - uca3L(2)) * l3u3intrs_temp1 / l3u3intrs_temp2
		};

		// scrub radius
		float scrubRadius_temp1 =
			(wcn[0] - spn[0]) * (l3u3intrs[0] - cpL[0]) +
			(wcn[1] - spn[1]) * (l3u3intrs[1] - cpL[1]) +
			(wcn[2] - spn[2]) * (l3u3intrs[2] - cpL[2]);

		float scrubRadius_temp2 =
			pow((wcn[0] - spn[0]), 2) +
			pow((wcn[1] - spn[1]), 2) +
			pow((wcn[2] - spn[2]), 2);

		scrubRadius = scrubRadius_temp1 / sqrtf(scrubRadius_temp2);

		return scrubRadius;

	}

	float GetKingpinAngle(int vertPos, int steerPos)
	{
		float kingpinAngle;

		int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		int Rv = 2 * vertIncr - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;									  // Left wheel position including vertical and steering movement
		int Rvs = (2 * vertIncr - vertPos) * (2 * steerIncr + 1) + 2 * steerIncr - steerPos;  // Right wheel position including vertical and steering movement

		//int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		//int Rv = cpGlob.rows() - 1 - vertPos;		// Right wheel only vertical position, UCA3, LCA3 points

		//int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;							// Left wheel position including vertical and steering movement
		//int Rvs = (cpGlob.rows() - 1 - vertPos) * (2 * steerIncr + 1) + steerPos;		// Right wheel position including vertical and steering movement

		Eigen::Vector3f cpL{ cpGlob.row(Lvs) };
		Eigen::Vector3f cpR{ cpGlob.row(Rvs) };

		Eigen::Vector3f grndNormal{
			0,
			-cpR(2) + cpL(2),
			-cpR(1) - cpL(1)
		};

		Eigen::Vector3f l3u3pr{
			0,
			-uca3Glob.row(Lv)(1) + lca3Glob.row(Lv)(1),
			-uca3Glob.row(Lv)(2) + lca3Glob.row(Lv)(2)
		};

		// if uca3 is closer to chassis centre then kingpin angle is positive
		if (abs(uca3Glob.row(Lv)(1)) < abs(lca3Glob.row(Lv)(1)))
		{
			kingpinAngle = acos(grndNormal.dot(l3u3pr) / grndNormal.norm() / l3u3pr.norm()) * 180.0f / 3.14159f;
			return kingpinAngle;
		}
		// otherwise negative kingpin angle
		else
		{
			kingpinAngle = -acos(grndNormal.dot(l3u3pr) / grndNormal.norm() / l3u3pr.norm()) * 180.0f / 3.14159f;
			return kingpinAngle;

		}
	}

	float GetAntiDrive(int vertPos, int steerPos)
	{
		float antiDrive;

		int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;							// Left wheel position including vertical and steering movement

		Eigen::Vector3f lca3{ lca3Glob.row(Lv) };
		Eigen::Vector3f uca3{ uca3Glob.row(Lv) };
		Eigen::Vector3f cp{ cpGlob.row(Lvs) };
		Eigen::Vector3f wcn{ wcnGlob.row(Lvs) };

		float aLCA;
		float aUCA;

		float bLCA;
		float bUCA;

		// LCA and UCA plane intersection with plane parallel to YZ plane with x coord CPR/2+CPL/2
		// lambda function that defines first point of intersection line
		auto intersectionLineCalc = [cp](float& aCa, float& bCa, const Eigen::Vector3f& ca1, const Eigen::Vector3f& ca2, const Eigen::Vector3f& ca3)
		{
			// plane coefficients Ax + By + Cz + D = 0, plane defined by control arm
			float A = (-ca1[1] + ca2[1]) * (-ca1[2] + ca3[2]) - (-ca1[1] + ca3[1]) * (-ca1[2] + ca2[2]);
			float B = -(-ca1[0] + ca2[0]) * (-ca1[2] + ca3[2]) + (-ca1[0] + ca3[0]) * (-ca1[2] + ca2[2]);
			float C = (-ca1[0] + ca2[0]) * (-ca1[1] + ca3[1]) - (-ca1[0] + ca3[0]) * (-ca1[1] + ca2[1]);
			float D = -ca1[0] * A - ca1[1] * B - ca1[2] * C;

			// intersection plane defined  as x + D2 = 0
			float D2 = -cp[1];

			aCa = -A / C;
			bCa = (B * D2 - D) / C;
		};

		intersectionLineCalc(aLCA, bLCA, lca1ref, lca2ref, lca3);
		intersectionLineCalc(aUCA, bUCA, uca1ref, uca2ref, uca3);

		float tanThetaOutboard;
		float tanThetaInboard;

		// if resulting lines are parallel
		if (abs(aLCA - aUCA) / abs(aLCA) < precision)
		{
			tanThetaInboard = aLCA;
			tanThetaOutboard = aLCA;
		}
		// if resulting lines are not parallel
		else
		{
			float ICPtx = (bLCA - bUCA) / (aUCA - aLCA);
			float ICPtz = aLCA * ICPtx + bLCA;

			tanThetaOutboard = (ICPtz - wcn[2]) / (ICPtx - wcn[0]);
			tanThetaInboard = (ICPtz - cp[2]) / (ICPtx - cp[0]);
		}

		if (suspPos == 0)  // front suspension
		{
			if (drivePos == 0)                        // outboard drive
				antiDrive = tanThetaOutboard * wheelbase / cogHeight * driveBias * 100;

			else if (driveBias == 0)                  // inboard drive
				antiDrive = 0;
			else
				antiDrive = tanThetaInboard * wheelbase / cogHeight / driveBias * 100;
		}

		else  // rear suspension
		{
			if (drivePos == 0)                        // outboard drive
				antiDrive = -tanThetaOutboard * wheelbase / cogHeight * driveBias * 100;

			else if (driveBias == 0)                  // inboard drive
				antiDrive = 0;
			else
				antiDrive = -tanThetaInboard * wheelbase / cogHeight / driveBias * 100;
		}

		return antiDrive;
	}

	float GetAntiBrake(int vertPos, int steerPos)
	{
		float antiBrakes;

		int Lv = vertPos;							// Left wheel only vertical position, UCA3, LCA3 points
		int Lvs = vertPos * (2 * steerIncr + 1) + steerPos;							// Left wheel position including vertical and steering movement



		Eigen::Vector3f lca3{ lca3Glob.row(Lv) };
		Eigen::Vector3f uca3{ uca3Glob.row(Lv) };
		Eigen::Vector3f cp{ cpGlob.row(Lvs) };
		Eigen::Vector3f wcn{ wcnGlob.row(Lvs) };

		float aLCA;
		float aUCA;

		float bLCA;
		float bUCA;

		// LCA and UCA plane intersection with plane parallel to YZ plane with x coord CPR/2+CPL/2
		// lambda function that defines first point of intersection line
		auto intersectionLineCalc = [cp](float& aCa, float& bCa, const Eigen::Vector3f& ca1, const Eigen::Vector3f& ca2, const Eigen::Vector3f& ca3)
		{
			// plane coefficients Ax + By + Cz + D = 0, plane defined by control arm
			float A = (-ca1[1] + ca2[1]) * (-ca1[2] + ca3[2]) - (-ca1[1] + ca3[1]) * (-ca1[2] + ca2[2]);
			float B = -(-ca1[0] + ca2[0]) * (-ca1[2] + ca3[2]) + (-ca1[0] + ca3[0]) * (-ca1[2] + ca2[2]);
			float C = (-ca1[0] + ca2[0]) * (-ca1[1] + ca3[1]) - (-ca1[0] + ca3[0]) * (-ca1[1] + ca2[1]);
			float D = -ca1[0] * A - ca1[1] * B - ca1[2] * C;

			// intersection plane defined  as x + D2 = 0
			float D2 = -cp[1];

			aCa = -A / C;
			bCa = (B * D2 - D) / C;
		};

		intersectionLineCalc(aLCA, bLCA, lca1ref, lca2ref, lca3);
		intersectionLineCalc(aUCA, bUCA, uca1ref, uca2ref, uca3);

		float tanThetaOutboard;
		float tanThetaInboard;

		// if resulting lines are parallel
		if (abs(aLCA - aUCA) / abs(aLCA) < precision)
		{
			tanThetaInboard = aLCA;
			tanThetaOutboard = aLCA;
		}
		// if resulting lines are not parallel
		else
		{
			float ICPtx = (bLCA - bUCA) / (aUCA - aLCA);
			float ICPtz = aLCA * ICPtx + bLCA;

			tanThetaOutboard = (ICPtz - wcn[2]) / (ICPtx - wcn[0]);
			tanThetaInboard = (ICPtz - cp[2]) / (ICPtx - cp[0]);
		}

		if (suspPos == 0)  // front suspension
		{
			if (brakePos == 0)                        // outboard brakes
				antiBrakes = tanThetaOutboard * wheelbase / cogHeight * brakeBias * 100;

			else if (brakeBias == 0)                  // inboard brakes
				antiBrakes = 0;
			else
				antiBrakes = tanThetaInboard * wheelbase / cogHeight / brakeBias * 100;
		}

		else  // rear suspension
		{
			if (brakePos == 0)                        // outboard brakes
				antiBrakes = -tanThetaOutboard * wheelbase / cogHeight * brakeBias * 100;

			else if (brakeBias == 0)                  // inboard brakes
				antiBrakes = 0;
			else
				antiBrakes = -tanThetaInboard * wheelbase / cogHeight / brakeBias * 100;

		}
		return antiBrakes;
	}

	float GetHalfTrackChange(int vertPos, int steerPos)
	{
		int position = vertPos * (2 * steerIncr + 1) + steerPos;
		float halfTrackChange;
		// if current wheelbase or half track is smaller than reference than negative sign, otherwise positive
		halfTrackChange = cpGlob.row(cpGlob.rows() / 2)[1] - cpGlob.row(position)[1];
		return halfTrackChange;
	}

	float GetWheelbaseChange(int vertPos, int steerPos)
	{
		int position = vertPos * (2 * steerIncr + 1) + steerPos;

		float wheelbaseChange;
		// if current wheelbase or half track is smaller than reference than negative sign, otherwise positive
		wheelbaseChange = -cpGlob.row(cpGlob.rows() / 2)[0] + cpGlob.row(position)[0];
		return wheelbaseChange;
	}

	float GetLca3DistanceFromWheelAxis()
	{
		return CalculateDistancePointToLine(spnref, wcnref, lca3ref);

	}

	float GetUca3DistanceFromWheelAxis()
	{
		return CalculateDistancePointToLine(spnref, wcnref, uca3ref);

	}

	float GetTr2DistanceFromWheelAxis()
	{
		return CalculateDistancePointToLine(spnref, wcnref, tr2ref);

	}

	float CalculateDistancePointToLine(const Eigen::Vector3f& linePt1, const Eigen::Vector3f& linePt2, const Eigen::Vector3f& Pt)
	{
		float distance;
		distance = (Pt - linePt1).cross(Pt - linePt2).norm() / (linePt2 - linePt1).norm();
		return distance;
	}

	float GetLca3DistanceToWheelCentrePlane()
	{
		return GetSignedPointToPlaneDistance(wcnref, spnref, lca3ref);
	}

	float GetUca3DistanceToWheelCentrePlane()
	{
		return GetSignedPointToPlaneDistance(wcnref, spnref, uca3ref);
	}

	float GetTr2DistanceToWheelCentrePlane()
	{
		return GetSignedPointToPlaneDistance(wcnref, spnref, tr2ref);
	}

	float GetSignedPointToPlaneDistance(const Eigen::Vector3f linePt1, const Eigen::Vector3f& linePt2, const Eigen::Vector3f& Pt)
	{
		/*Calculates distance from point to plane and gives a sign (+ or -) for distance, positive when distanced in direction of plane normal and  negative otherwise, linePt1 is the head of normal vector and linePt2 tail*/
		float distance;
		float A = linePt1[0] - linePt2[0];
		float B = linePt1[1] - linePt2[1];
		float C = linePt1[2] - linePt2[2];
		float D = -linePt1[0] * A - linePt1[1] * B - linePt1[2] * C;

		distance = (A * Pt[0] + B * Pt[1] + C * Pt[2] + D) / sqrtf(A * A + B * B + C * C);
		return distance;
	}

	void GetOptimisationCharacteristicsArray(float* characteristicsArray)
	{

		characteristicsArray[0] = GetCamberAngle(0, 0);
		characteristicsArray[1] = GetCamberAngle(2, 0);
		characteristicsArray[2] = GetToeAngle(0, 0);
		characteristicsArray[3] = GetToeAngle(2, 0);
		characteristicsArray[4] = GetCasterAngle(1);
		characteristicsArray[5] = GetRollCentreHeight(1, 0);
		characteristicsArray[6] = GetCasterTrail(1, 0);
		characteristicsArray[7] = GetScrubRadius(1, 0);
		characteristicsArray[8] = GetKingpinAngle(1, 0);
		characteristicsArray[9] = GetAntiDrive(1, 0);
		characteristicsArray[10] = GetAntiBrake(1, 0);
		characteristicsArray[11] = GetHalfTrackChange(0, 0);
		characteristicsArray[12] = GetHalfTrackChange(2, 0);
		characteristicsArray[13] = GetWheelbaseChange(0, 0);
		characteristicsArray[14] = GetWheelbaseChange(2, 0);
		characteristicsArray[15] = GetLca3DistanceFromWheelAxis();
		characteristicsArray[16] = GetUca3DistanceFromWheelAxis();
		characteristicsArray[17] = GetTr2DistanceFromWheelAxis();
		characteristicsArray[18] = GetLca3DistanceToWheelCentrePlane();
		characteristicsArray[19] = GetUca3DistanceToWheelCentrePlane();
		characteristicsArray[20] = GetTr2DistanceToWheelCentrePlane();

	}

	void GetMovedHardpoints(float* outputLca3, float* outputUca3, float* outputTr1, float* outputTr2,
		float* outputWcn, float* outputSpn) {
		
		for (int i = 0; i < vertIncr * 2 + 1; i++)
		{
			outputLca3[i * 3] = lca3Glob.row(i)(0);
			outputLca3[i * 3 + 1] = lca3Glob.row(i)(1);
			outputLca3[i * 3 + 2] = lca3Glob.row(i)(2);
			
			outputUca3[i * 3] = uca3Glob.row(i)(0);
			outputUca3[i * 3 + 1] = uca3Glob.row(i)(1);
			outputUca3[i * 3 + 2] = uca3Glob.row(i)(2);

			for (int j = 0; j < steerIncr * 2 + 1; j++)
			{
				outputTr2[(i * (2 * steerIncr + 1) + j) * 3] = tr2Glob.row(i * (2 * steerIncr + 1) + j)(0);
				outputTr2[(i * (2 * steerIncr + 1) + j) * 3 + 1] = tr2Glob.row(i * (2 * steerIncr + 1) + j)(1);
				outputTr2[(i * (2 * steerIncr + 1) + j) * 3 + 2] = tr2Glob.row(i * (2 * steerIncr + 1) + j)(2);

				outputWcn[(i * (2 * steerIncr + 1) + j) * 3] = wcnGlob.row(i * (2 * steerIncr + 1) + j)(0);
				outputWcn[(i * (2 * steerIncr + 1) + j) * 3 + 1] = wcnGlob.row(i * (2 * steerIncr + 1) + j)(1);
				outputWcn[(i * (2 * steerIncr + 1) + j) * 3 + 2] = wcnGlob.row(i * (2 * steerIncr + 1) + j)(2);

				outputSpn[(i * (2 * steerIncr + 1) + j) * 3] = spnGlob.row(i * (2 * steerIncr + 1) + j)(0);
				outputSpn[(i * (2 * steerIncr + 1) + j) * 3 + 1] = spnGlob.row(i * (2 * steerIncr + 1) + j)(1);
				outputSpn[(i * (2 * steerIncr + 1) + j) * 3 + 2] = spnGlob.row(i * (2 * steerIncr + 1) + j)(2);
			}
		}

		for (int j = 0; j < steerIncr * 2 + 1; j++)
		{
			outputTr1[j * 3] = tr1Glob.row(j)(0);
			outputTr1[j * 3 + 1] = tr1Glob.row(j)(1);
			outputTr1[j * 3 + 2] = tr1Glob.row(j)(2);
		}
	}
};


void optimisation_obj_res(float* hardpoints, int suspPos, float wRadius,
	float wheelbase, float cogHeight, float driveBias, float brakeBias,
	int drivePos, int brakePos,
	float wVert, float* peakWidth, float* flatness, float wSteer, int vertIncr, int steerIncr, float precision, float* targetValues,
	float* weightFactors, float& obj_func_res, float* outputParams)
{
	Suspension susp{

		hardpoints,
		wRadius,
		wheelbase, cogHeight, driveBias, brakeBias,
		suspPos, drivePos, brakePos,
		wVert, wSteer,
		vertIncr, steerIncr, precision
	};
	susp.CalculateMovement();


	susp.GetOptimisationCharacteristicsArray(outputParams);
	obj_func_res = susp.GetObjFuncScore(peakWidth, flatness, outputParams, targetValues, weightFactors);
}



void suspension_movement(float* hardpoints, float wRadiusin,
	float wheelbase, float cogHeight, float frontDriveBias, float frontBrakeBias,
	int suspPos, int drivePos, int brakePos,
	float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin, 

	float* camberAngle, 
	float* toeAngle, 
	float* casterAngle, 
	float* rcHeight, 
	float* casterTrail,
	float* scrubRadius,
	float* kingpinAngle,
	float* antiDrive, 
	float* antiBrake, 
	float* halfTrackChange,
	float* wheelbaseChange, 
	float* constOutputParams,

	float* outputLca3, 
	float* outputUca3, 
	float* outputTr1, 
	float* outputTr2, 
	float* outputWcn, 
	float* outputSpn)
{

	Suspension susp{
		hardpoints,
		wRadiusin,
		wheelbase, cogHeight, frontDriveBias, frontBrakeBias,
		suspPos, drivePos, brakePos,
		wVertin, wSteerin,
		vertIncrin, steerIncrin, precisionin
	};

	susp.CalculateMovement();

	susp.GetMovedHardpoints(outputLca3, outputUca3, outputTr1, outputTr2, outputWcn, outputSpn);
	susp.LogToConsole();
	for (int i = 0; i < vertIncrin * 2 + 1; i++)
	{

		for (int j = 0; j < steerIncrin * 2 + 1; j++)
		{
			camberAngle[i * (2 * steerIncrin + 1) + j] = susp.GetCamberAngle(i, j);
			std::cout << "camber angle " << susp.GetCamberAngle(i, j)<<"\n";
			toeAngle[i * (2 * steerIncrin + 1) + j] = susp.GetToeAngle(i, j);
			casterAngle[i * (2 * steerIncrin + 1) + j] = susp.GetCasterAngle(i);
			rcHeight[i * (2 * steerIncrin + 1) + j] = susp.GetRollCentreHeight(i, j);
			casterTrail[i * (2 * steerIncrin + 1) + j] = susp.GetCasterTrail(i, j);
			scrubRadius[i * (2 * steerIncrin + 1) + j] = susp.GetScrubRadius(i, j);
			kingpinAngle[i * (2 * steerIncrin + 1) + j] = susp.GetKingpinAngle(i, j);
			antiDrive[i * (2 * steerIncrin + 1) + j] = susp.GetAntiDrive(i, j);
			antiBrake[i * (2 * steerIncrin + 1) + j] = susp.GetAntiBrake(i, j);
			halfTrackChange[i * (2 * steerIncrin + 1) + j] = susp.GetHalfTrackChange(i, j);
			wheelbaseChange[i * (2 * steerIncrin + 1) + j] = susp.GetWheelbaseChange(i, j);

		}
	}

	constOutputParams[0] = susp.GetLca3DistanceFromWheelAxis();
	constOutputParams[1] = susp.GetUca3DistanceFromWheelAxis();
	constOutputParams[2] = susp.GetTr2DistanceFromWheelAxis();
	constOutputParams[3] = susp.GetLca3DistanceToWheelCentrePlane();
	constOutputParams[4] = susp.GetUca3DistanceToWheelCentrePlane();
	constOutputParams[5] = susp.GetTr2DistanceToWheelCentrePlane();
}
