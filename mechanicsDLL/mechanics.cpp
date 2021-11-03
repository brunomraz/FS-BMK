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



	// wheel radius
	float wRadius = 210;
	// wheel vertical movement
	float wVert = 30;
	// wheel steering movement
	float wSteer = 30;
	// number of increments between reference position and
	// downmost/upmost/leftmost/rightmost position
	int vertIncr = 30;
	int steerIncr = 0;
	// precision- at what value has the iterator converged, in percentage- 0...1
	float precision;

	float wheelbase;
	float cogHeight;
	float frontDriveBias;
	float rearDriveBias{ 1.0f - frontDriveBias };

	float frontBrakeBias;
	float rearBrakeBias{ 1.0f - frontBrakeBias };

	// front or rear suspension 0 for front, 1 for rear
	int suspPos;
	// outboard or inboard drive 0 for outboard, 1 for inboard
	int drivePos;
	// outboard or inboard brakes 0 for outboard, 1 for inboard
	int brakePos;


	// derived values

	Eigen::Vector3f lca12;
	Eigen::Vector3f uca12;
	Eigen::MatrixXf lca3Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf uca3Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf tr2Glob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf wcnGlob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf spnGlob;// (vertIncr * 2 + 1, 3);
	Eigen::MatrixXf cpGlob;// (vertIncr * 2 + 1, 3);


	/* initializers */

public:


	Suspension()
	{
		std::vector<float> hps;
		std::string userArrayInput;
		std::cout << "enter numbers: ";
		std::cin >> userArrayInput;

		std::istringstream iss(userArrayInput);
		std::string item;

		while (std::getline(iss, item, ','))
		{
			hps.push_back(std::stof(item));
		}

		std::cout << "for loop vector output" << std::endl;

		for (auto i = hps.begin(); i != hps.end(); i++)
			std::cout << *i << std::endl;

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

	}

	Suspension(
		float* hps,
		float wRadiusin,
		float wheelbasein, float cogHeightin, float frontDriveBiasin,
		float frontBrakeBiasin, int suspPosin, int drivePosin, int brakePosin,
		float wVertin, float wSteerin,
		int vertIncrin, int steerIncrin, float precisionin) :lca3Glob(vertIncrin * 2 + 1, 3), uca3Glob(vertIncrin * 2 + 1, 3), tr2Glob(vertIncrin * 2 + 1, 3), wcnGlob(vertIncrin * 2 + 1, 3), spnGlob(vertIncrin * 2 + 1, 3), cpGlob(vertIncrin * 2 + 1, 3)
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
		frontDriveBias = frontDriveBiasin;
		rearDriveBias = 1.0f - frontDriveBias;

		frontBrakeBias = frontBrakeBiasin;
		rearBrakeBias = 1.0f - frontBrakeBias;

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




		// calculating TR2, WCN, SPN and CP
		// steering is not enabled, for optimization
		// condition should be if (steerIncr = 0 || wSteer = 0)
		if (steerIncr == 0 || wSteer == 0)
		{

			// calculating TR2 position

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


				// calculate local position of TR1
				Eigen::Vector3f tr1locTR;

				//lca3LocUCA = lca3Glob.rowwise() - uca12.transpose();
				//lca3LocUCA = lca3LocUCA * rotUCA;

				tr1locTR = -tr2prGlob.row(i) + tr1ref.transpose();
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

				tr2Glob.row(i) << (rotTR * tr2locTR).transpose() + tr2prGlob.row(i);



				// calculating WCN and SPN 
				Eigen::Matrix3f _rotTRk; // TR rotation matrix defined by TR2ref

				Eigen::Vector3f _xCol{ tr2prGlob.row(i) - tr2Glob.row(i) };
				Eigen::Vector3f _zCol{ lca3Glob.row(i) - uca3Glob.row(i) };
				Eigen::Vector3f _yCol{ _zCol.cross(_xCol) };


				_rotTRk.col(0) << _xCol / _xCol.norm();
				_rotTRk.col(1) << _yCol / _yCol.norm();
				_rotTRk.col(2) << _zCol / _zCol.norm();

				wcnGlob.row(i) << (_rotTRk * wcnlocTRk).transpose() + tr2prGlob.row(i);
				spnGlob.row(i) << (_rotTRk * spnlocTRk).transpose() + tr2prGlob.row(i);

			}

			// CP calculation
			float temp1cp{ -20 }; // this is actually vector 0,0,-20
			Eigen::MatrixXf temp2cp(vertIncr * 2 + 1, 3);
			Eigen::MatrixXf temp3cp(vertIncr * 2 + 1, 3);

			temp2cp << spnGlob - wcnGlob;

			temp3cp.col(0) << -temp2cp.col(0).array() * temp2cp.col(2).array() * temp1cp;
			temp3cp.col(1) << -temp2cp.col(1).array() * temp2cp.col(2).array() * temp1cp;
			temp3cp.col(2) <<
				temp2cp.col(1).array() * temp2cp.col(1).array() * temp1cp +
				temp2cp.col(0).array() * temp2cp.col(0).array() * temp1cp;

			temp3cp.rowwise().normalize();


			cpGlob << -temp3cp * wRadius + wcnGlob;




			/* output params :
			0  objective function
			1  camber angle down
			2 			    up
			3  toe angle down
			4 		     up
			5  caster angle
			6  roll centre height
			7  caster trail
			8  scrub radius
			9  kingpin angle
			10 anti squat / anti dive drive
			11 anti rise / anti lift  brake
			12 half track change down
			13 wheelbase change down
			14 half track change up
			15 wheelbase change up
			16 distance lca3 to wcn-spn line
			17 distance uca3 to wcn-spn line
			18 distance tr2 to wcn-spn line
			19 distance lca3 to plane with wcn-spn normal through wcn point
			20 distance uca3 to plane with wcn-spn normal through wcn point
			21 distance tr2 to plane with wcn-spn normal through wcn point
			*/

		}

		// steering is enabled
		else
		{
		}
	}

	float GetObjCamberScore()
	{
		float camberScore;
		float camberUp;
		float camberDown;
		camberUp = GetCamberAngle(2);
		camberDown = GetCamberAngle(0);
		float wantedCamberUp = -0.978327f;
		float wantedCamberDown = -2.65318f;
		float peakWidth = 100.0f;
		float camberUpObj{ (float)exp(-peakWidth * pow(camberUp - wantedCamberUp,2)) * 0.5f };
		float camberDownObj{ (float)exp(-peakWidth * pow(camberDown - wantedCamberDown,2)) * 0.5f };

		camberScore = 1 - camberUpObj - camberDownObj;
		return camberScore;
	}

	float GetCamberAngle(int position)
	{
		float camberAngle;
		int L = position;
		int R = cpGlob.rows() - 1 - position;


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

	float GetToeAngle(int position)
	{
		float toeAngle;
		// positive toe angle for toe in and negative for toe out
		Eigen::Vector3f wheelAxis = wcnGlob.row(position) - spnGlob.row(position);
		Eigen::Vector3f refAxis{
			0,
			wcnGlob.row(position)(1) - spnGlob.row(position)(1),
			wcnGlob.row(position)(2) - spnGlob.row(position)(2)
		};

		float toe = acos(refAxis.norm() / wheelAxis.norm());

		if (wcnGlob.row(position)(0) < spnGlob.row(position)(0)) // toe out case
		{
			toeAngle = -toe * 180 / 3.14159f;
			return toeAngle;
		}

		else // toe in case
		{
			toeAngle = toe * 180 / 3.14159f;
			return toeAngle;

		}

	}

	float GetCasterAngle(int position)
	{
		float casterAngle;
		float caster =
			atan2f(
				(lca3Glob.row(position)(0) - uca3Glob.row(position)(0))
				, (-uca3Glob.row(position)(2) + lca3Glob.row(position)(2)));

		casterAngle = caster * 180 / 3.14159f;
		return casterAngle;
	}

	float GetRollCentreHeight(int position)
	{
		float rollCentreHeight;
		int L = position;                  // L means left
		int R = cpGlob.rows() - 1 - position;  // R means right

		float slopePrecision{ 0.001f }; // if difference between slopes is less than this value, than they are considered parallel

		Eigen::Vector3f lca3L{ lca3Glob.row(L) };
		Eigen::Vector3f uca3L{ uca3Glob.row(L) };
		Eigen::Vector3f cpL{ cpGlob.row(L) };

		Eigen::Vector3f lca1R{ lca1ref(0),-lca1ref(1),lca1ref(2) };
		Eigen::Vector3f lca2R{ lca2ref(0),-lca2ref(1),lca2ref(2) };
		Eigen::Vector3f lca3R{ lca3Glob.row(R)(0),-lca3Glob.row(R)(1),lca3Glob.row(R)(2) };

		Eigen::Vector3f uca1R{ uca1ref(0),-uca1ref(1),uca1ref(2) };
		Eigen::Vector3f uca2R{ uca2ref(0),-uca2ref(1),uca2ref(2) };
		Eigen::Vector3f uca3R{ uca3Glob.row(R)(0),-uca3Glob.row(R)(1),uca3Glob.row(R)(2) };
		Eigen::Vector3f cpR{ cpGlob.row(R)(0),-cpGlob.row(R)(1),cpGlob.row(R)(2) };

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

		// case if LEFT LCA and UCA are parallel
		if (abs(aLCAL - aUCAL) < slopePrecision)
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


		// case if RIGHT LCA and UCA are parallel
		if (abs(aLCAR - aUCAR) < slopePrecision)
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

		if (abs((aICR - aICL)) < precision)
		{
			rollCentreHeight = 0;
			return rollCentreHeight;
		}

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

	float GetCasterTrail(int position)
	{
		float casterTrail;
		int L = position;                  // L means left
		int R = cpGlob.rows() - 1 - position;  // R means right

		Eigen::Vector3f cpL{ cpGlob.row(L) };
		Eigen::Vector3f cpR{ cpGlob.row(R) };
		Eigen::Vector3f wcn{ wcnGlob.row(L) };
		Eigen::Vector3f spn{ spnGlob.row(L) };
		Eigen::Vector3f lca3L{ lca3Glob.row(L) };
		Eigen::Vector3f uca3L{ uca3Glob.row(L) };

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

	float GetScrubRadius(int position)
	{
		float scrubRadius;
		int L = position;                  // L means left
		int R = cpGlob.rows() - 1 - position;  // R means right

		Eigen::Vector3f cpL{ cpGlob.row(L) };
		Eigen::Vector3f cpR{ cpGlob.row(R) };
		Eigen::Vector3f wcn{ wcnGlob.row(L) };
		Eigen::Vector3f spn{ spnGlob.row(L) };
		Eigen::Vector3f lca3L{ lca3Glob.row(L) };
		Eigen::Vector3f uca3L{ uca3Glob.row(L) };

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

	float GetKingpinAngle(int position)
	{
		float kingpinAngle;
		int L = position;                  // L means left
		int R = cpGlob.rows() - 1 - position;  // R means right

		Eigen::Vector3f cpL{ cpGlob.row(L) };
		Eigen::Vector3f cpR{ cpGlob.row(R) };

		Eigen::Vector3f grndNormal{
			0,
			-cpR(2) + cpL(2),
			-cpR(1) - cpL(1)

		};

		Eigen::Vector3f l3u3pr{
			0,
			-uca3Glob.row(L)(1) + lca3Glob.row(L)(1),
			-uca3Glob.row(L)(2) + lca3Glob.row(L)(2)
		};

		// if uca3 is closer to chassis centre then kingpin angle is positive
		if (abs(uca3Glob.row(L)(1)) < abs(lca3Glob.row(L)(1)))
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

	void GetAntiFeatures(float& antiDrive, float& antiBrakes, int position)
	{
		int L = position;

		Eigen::Vector3f lca3{ lca3Glob.row(L) };
		Eigen::Vector3f uca3{ uca3Glob.row(L) };
		Eigen::Vector3f cp{ cpGlob.row(position) };
		Eigen::Vector3f wcn{ wcnGlob.row(position) };

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
		if (abs(aLCA - aUCA) < precision)
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

		// front suspension
		if (suspPos == 0)
		{
			if (drivePos == 0)     // outboard drive
				antiDrive = tanThetaOutboard * wheelbase / cogHeight * frontDriveBias * 100;

			else                   // inboard drive
				antiDrive = tanThetaInboard * wheelbase / cogHeight / frontDriveBias * 100;


			if (brakePos == 0)       // outboard brakes
				antiBrakes = tanThetaOutboard * wheelbase / cogHeight * frontBrakeBias * 100;

			else                   // inboard brakes
				antiBrakes = tanThetaInboard * wheelbase / cogHeight / frontBrakeBias * 100;
		}
		// rear suspension
		else
		{
			if (drivePos == 0)     // outboard drive
				antiDrive = -tanThetaOutboard * wheelbase / cogHeight * rearDriveBias * 100;

			else                   // inboard drive
				antiDrive = -tanThetaInboard * wheelbase / cogHeight / rearDriveBias * 100;

			if (brakePos == 0)     // outboard brakes
				antiBrakes = -tanThetaOutboard * wheelbase / cogHeight * rearBrakeBias * 100;

			else                   // inboard brakes
				antiBrakes = -tanThetaInboard * wheelbase / cogHeight / rearBrakeBias * 100;

		}
	}

	void GetHalfTrackAndWheelbaseChange(float& halfTrackChange, float& wheelbaseChange, int position)
	{
		// if current wheelbase or half track is smaller than reference than negative sign, otherwise positive
		halfTrackChange = cpGlob.row(cpGlob.rows() / 2)[1] - cpGlob.row(position)[1];
		wheelbaseChange = -cpGlob.row(cpGlob.rows() / 2)[0] + cpGlob.row(position)[0];
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

	void GetMovedHardpoints(float* movedHardpoints) {
		movedHardpoints[0] = lca3Glob.row(0)(0), movedHardpoints[1] = lca3Glob.row(0)(1), movedHardpoints[2] = lca3Glob.row(0)(2);
		movedHardpoints[3] = uca3Glob.row(0)(0), movedHardpoints[4] = uca3Glob.row(0)(1), movedHardpoints[5] = uca3Glob.row(0)(2);
		movedHardpoints[6] = tr2Glob.row(0)(0), movedHardpoints[7] = tr2Glob.row(0)(1), movedHardpoints[8] = tr2Glob.row(0)(2);
		movedHardpoints[9] = wcnGlob.row(0)(0), movedHardpoints[10] = wcnGlob.row(0)(1), movedHardpoints[11] = wcnGlob.row(0)(2);
		movedHardpoints[12] = spnGlob.row(0)(0), movedHardpoints[13] = spnGlob.row(0)(1), movedHardpoints[14] = spnGlob.row(0)(2);
	}

};


float optimisation_obj_res(float* hardpoints, float wRadiusin,
	float wheelbase, float cogHeight, float frontDriveBias, float frontBrakeBias,
	int suspPos, int drivePos, int brakePos,
	float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin, float* outputParams)
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

	outputParams[0] = susp.GetObjCamberScore();
	outputParams[1] = susp.GetCamberAngle(0);
	outputParams[2] = susp.GetCamberAngle(2);
	outputParams[3] = susp.GetToeAngle(0);
	outputParams[4] = susp.GetToeAngle(2);
	outputParams[5] = susp.GetCasterAngle(1);
	outputParams[6] = susp.GetRollCentreHeight(1);
	outputParams[7] = susp.GetCasterTrail(1);
	outputParams[8] = susp.GetScrubRadius(1);
	outputParams[9] = susp.GetKingpinAngle(1);
	susp.GetAntiFeatures(outputParams[10], outputParams[11], 1);
	susp.GetHalfTrackAndWheelbaseChange(outputParams[12], outputParams[13], 0);
	susp.GetHalfTrackAndWheelbaseChange(outputParams[14], outputParams[15], 2);
	// for constraints inside wheel
	outputParams[16] = susp.GetLca3DistanceFromWheelAxis();
	outputParams[17] = susp.GetUca3DistanceFromWheelAxis();
	outputParams[18] = susp.GetTr2DistanceFromWheelAxis();

	outputParams[19] = susp.GetLca3DistanceToWheelCentrePlane();
	outputParams[20] = susp.GetUca3DistanceToWheelCentrePlane();
	outputParams[21] = susp.GetTr2DistanceToWheelCentrePlane();

	return 1.0f;
}



void suspension_movement(float* hardpoints, float wRadiusin,
	float wheelbase, float cogHeight, float frontDriveBias, float frontBrakeBias,
	int suspPos, int drivePos, int brakePos,
	float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin, float* outputParams, float* outputHardpoints)
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

	susp.GetMovedHardpoints(outputHardpoints);

	outputParams[0] = susp.GetCamberAngle(0);
	outputParams[1] = susp.GetToeAngle(0);
	outputParams[2] = susp.GetCasterAngle(0);
	outputParams[3] = susp.GetRollCentreHeight(0);
	outputParams[4] = susp.GetCasterTrail(0);
	outputParams[5] = susp.GetScrubRadius(0);
	outputParams[6] = susp.GetKingpinAngle(0);
	susp.GetAntiFeatures(outputParams[7], outputParams[8], 0);
	susp.GetHalfTrackAndWheelbaseChange(outputParams[9], outputParams[10], 0);


}
