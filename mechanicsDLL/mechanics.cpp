#include "pch.h"
#include "mechanics.h"
#include <iostream>
#include <Eigen/Dense>
#include <Eigen/Geometry>
#include <vector>
#include <sstream>
#include <math.h>



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

	// derived values

	Eigen::Vector3f lca12;
	Eigen::Vector3f uca12;


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
		float lca1xin, float lca1yin, float lca1zin, // in suffix stands for input
		float lca2xin, float lca2yin, float lca2zin,
		float lca3xin, float lca3yin, float lca3zin,
		float uca1xin, float uca1yin, float uca1zin,
		float uca2xin, float uca2yin, float uca2zin,
		float uca3xin, float uca3yin, float uca3zin,
		float tr1xin, float tr1yin, float tr1zin,
		float tr2xin, float tr2yin, float tr2zin,
		float wcnxin, float wcnyin, float wcnzin,
		float spnxin, float spnyin, float spnzin,
		float wRadiusin, float wVertin, float wSteerin,
		int vertIncrin, int steerIncrin, float precisionin)
	{
		lca1ref << lca1xin, lca1yin, lca1zin;
		lca2ref << lca2xin, lca2yin, lca2zin;
		lca3ref << lca3xin, lca3yin, lca3zin;

		uca1ref << uca1xin, uca1yin, uca1zin;
		uca2ref << uca2xin, uca2yin, uca2zin;
		uca3ref << uca3xin, uca3yin, uca3zin;

		tr1ref << tr1xin, tr1yin, tr1zin;
		tr2ref << tr2xin, tr2yin, tr2zin;

		wcnref << wcnxin, wcnyin, wcnzin;
		spnref << spnxin, spnyin, spnzin;

		wRadius = wRadiusin;
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

	void CalculateConstants(Eigen::Matrix3f& _rotLCA, Eigen::Matrix3f& _rotUCA, 
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

		_t_param = (_tr2prref(0) - lca3ref(0)) / (uca3ref(0) - lca3ref(0));


		Eigen::Matrix3f _rotTRk; // TR rotation matrix defined by TR2ref

		Eigen::Vector3f _xCol{ _tr2prref - tr2ref };
		Eigen::Vector3f _zCol{ lca3ref - uca3ref };
		Eigen::Vector3f _yCol{ _zCol.cross(_xCol) };


		_rotTRk.col(0) << _xCol / _xCol.norm();
		_rotTRk.col(1) << _yCol / _yCol.norm();
		_rotTRk.col(2) << _zCol / _zCol.norm();

		_wcnlocTRk << _rotTRk.transpose() * (wcnref - _tr2prref);
		_spnlocTRk << _rotTRk.transpose() * (spnref - _tr2prref);

		std::cout << "spnlocTRk" << std::endl;
		std::cout << _spnlocTRk << std::endl;
	}

public:

	float CalculateMovement()
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
		Eigen::MatrixXf lca3Glob(vertIncr * 2 + 1, 3);

		// populating positions of local LCA3 in a matrix
		lca3LocLCA.col(0) << Eigen::VectorXf::Zero(vertIncr * 2 + 1);
		lca3LocLCA.col(1) << -(rLCA * rLCA - zLCA3LocLCA * zLCA3LocLCA).sqrt();
		lca3LocLCA.col(2) << zLCA3LocLCA;

		// global positions of LCA3 for whole wheel movement
		lca3Glob = (lca3LocLCA * rotLCA.transpose()).array().rowwise() + lca12.array().transpose();

		// global position of UCA3 for whole wheel movement
		Eigen::MatrixXf uca3Glob(vertIncr * 2 + 1, 3);
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
			lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array() * lca3LocUCA.col(1).array()  -
			lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array() * lca3LocUCA.col(2).array();

		Eigen::ArrayXf temp7UCA3 = (temp3UCA3 + temp4UCA3 + temp5UCA3 + temp6UCA3).sqrt();

		uca3LocUCA.col(0) << Eigen::VectorXf::Zero(vertIncr * 2 + 1);
		uca3LocUCA.col(1) << (lca3LocUCA.col(1).array() * temp1UCA3 - lca3LocUCA.col(2).array() * temp7UCA3) / temp2UCA3;
		uca3LocUCA.col(2) << (lca3LocUCA.col(2).array() * temp1UCA3 + lca3LocUCA.col(1).array() * temp7UCA3) / temp2UCA3;


		uca3Glob = (uca3LocUCA * rotUCA.transpose()).array().rowwise() + uca12.array().transpose();




		// calculating TR2, WCN, SPN and CP
		// steering is not enabled, for optimization
		if (steerIncr != 0 || wSteer != 0)
		{

			std::cout << "no steer movement" << std::endl;
			// calculating TR2 position
			Eigen::MatrixXf tr2Glob(vertIncr * 2 + 1, 3);
			Eigen::MatrixXf wcnGlob(vertIncr * 2 + 1, 3);
			Eigen::MatrixXf spnGlob(vertIncr * 2 + 1, 3);

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
					 - 2 * tr1locTR(1) * tr1locTR(1) * tr1locTR(2) * tr1locTR(2) -
					tr1locTR(1) * tr1locTR(1) * tr1locTR(1) * tr1locTR(1) -
					tr1locTR(2) * tr1locTR(2) * tr1locTR(2) * tr1locTR(2);

				float temp7TR2 = std::sqrt(temp3TR2 + temp4TR2 + temp5TR2 + temp6TR2);


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

			std::cout << "temp2cp  " << std::endl;
			std::cout << temp2cp << std::endl;

			temp3cp.col(0) << -temp2cp.col(0).array() * temp2cp.col(2).array() * temp1cp;
			temp3cp.col(1) << -temp2cp.col(1).array() * temp2cp.col(2).array() * temp1cp;
			temp3cp.col(2) << 
				temp2cp.col(1).array() * temp2cp.col(1).array() * temp1cp + 
				temp2cp.col(0).array() * temp2cp.col(0).array() * temp1cp;

			temp3cp.rowwise().normalize();

			Eigen::MatrixXf cpGlob(vertIncr * 2 + 1, 3);

			cpGlob << -temp3cp * wRadius + wcnGlob;



			std::cout << "caster trail________" << 
				CalculateCasterTrail(lca3Glob, uca3Glob, spnGlob, wcnGlob, cpGlob, 0)
				<<"  " << 
				CalculateCasterTrail(lca3Glob, uca3Glob, spnGlob, wcnGlob, cpGlob, 2)
				<<"  score  "<< 
				CalculateObjCamberScore(cpGlob, wcnGlob, spnGlob) << '\n';

			return CalculateObjCamberScore(cpGlob, wcnGlob, spnGlob);

			

		}

		// steering is enabled
		else
		{
			return 456;
		}


	}

	float CalculateObjCamberScore(Eigen::MatrixXf& cp, Eigen::MatrixXf& wcn, Eigen::MatrixXf& spn)
	{
		float camberUp{ CalculateCamber(cp, wcn, spn, 2) };
		float camberDown{ CalculateCamber(cp, wcn, spn, 0) };

		float wantedCamberUp = -2.65318;
		float wantedCamberDown = -0.978327;

		float peakWidth = 100;

		float camberUpObj{ (float)exp(-peakWidth * pow(camberUp - wantedCamberUp,2))*0.5f };
		float camberDownObj{ (float)exp(-peakWidth * pow(camberDown - wantedCamberDown,2)) *0.5f};

		float objectiveSum = 1 - camberUpObj - camberDownObj;

		return objectiveSum;


	}

	float CalculateCamber(Eigen::MatrixXf& cp, Eigen::MatrixXf& wcn, Eigen::MatrixXf& spn, int position)
	{
		
		int L = position;
		int R = cp.rows() - 1 - position;
				

		Eigen::Vector3f wheelAxis{
			-wcn.row(L)(0) + cp.row(L)(0),
			-wcn.row(L)(1) + cp.row(L)(1),
			-wcn.row(L)(2) + cp.row(L)(2)
		};
		Eigen::Vector3f groundNormal{
			0,
			-cp.row(R)(2) + cp.row(L)(2),
			-cp.row(R)(1) - cp.row(L)(1)
		};
		// calculate plane parallel to ground going through SPN point with respect to which camber is measured
		
		float temp1_wcnpr =
			spn.row(L)(1) * groundNormal(1)
			+ spn.row(L)(2) * groundNormal(2)
			- wcn.row(L)(1) * groundNormal(1)
			- wcn.row(L)(2) * groundNormal(2);

		float temp2_wcnpr =
			groundNormal(1) * groundNormal(1) + 
			groundNormal(2) * groundNormal(2);

		Eigen::Vector3f wcnpr{
			wcn.row(L)(0),
			wcn.row(L)(1) + groundNormal(1) * temp1_wcnpr / temp2_wcnpr,
			wcn.row(L)(2) + groundNormal(2) * temp1_wcnpr / temp2_wcnpr
		};


		float camber =
			(wcnpr - (Eigen::Vector3f)wcn.row(L)).norm() /
			((Eigen::Vector3f)spn.row(L) - 
				(Eigen::Vector3f)wcn.row(L)).norm();


		
		// tests if camber is negative, if it is it returns negative angle
		if ((wcnpr - (Eigen::Vector3f)wcn.row(L))(2) > 0)
			return -asin(camber) * 180 / 3.14159f;
		// if camber is not negative, returns positive angle
		else
			return asin(camber) * 180 / 3.14159f;

		 
	}

	float CalculateToe(Eigen::MatrixXf& cp, Eigen::MatrixXf& wcn, Eigen::MatrixXf& spn, int position)
	{
		// positive toe angle for toe in and negative for toe out
		Eigen::Vector3f wheelAxis = wcn.row(position) - spn.row(position);
		Eigen::Vector3f refAxis{
			0,
			wcn.row(position)(1) - spn.row(position)(1),
			wcn.row(position)(2) - spn.row(position)(2)
		};

		std::cout << "wheel axis coords_________ ";
		std::cout << wheelAxis << "\n";
		std::cout << "wheel axis norm_________ ";
		std::cout << wheelAxis.norm() << "\n";


		std::cout << "ref axis coords_________ ";
		std::cout << refAxis << "\n";
		std::cout << "ref axis norm_________ ";
		std::cout << refAxis.norm() << "\n";

		float toe = acos(refAxis.norm() / wheelAxis.norm());
		
		if (wcn.row(position)(0) < spn.row(position)(0)) // toe out case
			return -toe * 180 / 3.14159f;
		else // toe in case
			return toe * 180 / 3.14159f;

	}

	float CalculateCaster(Eigen::MatrixXf& lca3, Eigen::MatrixXf& uca3, int position)
	{
		float caster =
			atan2f(
				(-lca3.row(position)(0) + uca3.row(position)(0))
				, (-uca3.row(position)(2) + lca3.row(position)(2)));
		return caster * 180 / 3.14159f;
	}

	float CalculateRollCentreHeight(Eigen::Vector3f& lca1L, Eigen::Vector3f& lca2L, Eigen::MatrixXf& lca3Lmat, Eigen::Vector3f& uca1L, Eigen::Vector3f& uca2L, Eigen::MatrixXf& uca3Lmat, Eigen::MatrixXf& cpLmat, int position)
	{

		int L = position;                  // L means left
		int R = cpLmat.rows() - 1 - position;  // R means right

		float slopePrecision{ 0.001f }; // if difference between slopes is less than this value, than they are considered parallel

		Eigen::Vector3f lca3L{ lca3Lmat.row(L) };
		Eigen::Vector3f uca3L{ uca3Lmat.row(L) };
		Eigen::Vector3f cpL{ cpLmat.row(L) };

		Eigen::Vector3f lca1R{ lca1L(0),-lca1L(1),lca1L(2) };
		Eigen::Vector3f lca2R{ lca2L(0),-lca2L(1),lca2L(2) };
		Eigen::Vector3f lca3R{ lca3Lmat.row(R)(0),-lca3Lmat.row(R)(1),lca3Lmat.row(R)(2) };

		Eigen::Vector3f uca1R{ uca1L(0),-uca1L(1),uca1L(2) };
		Eigen::Vector3f uca2R{ uca2L(0),-uca2L(1),uca2L(2) };
		Eigen::Vector3f uca3R{ uca3Lmat.row(R)(0),-uca3Lmat.row(R)(1),uca3Lmat.row(R)(2) };
		Eigen::Vector3f cpR{ cpLmat.row(R)(0),-cpLmat.row(R)(1),cpLmat.row(R)(2) };


		Eigen::Vector3f lca12Lpr{
			cpL(0) / 2 + cpR(0) / 2,
			lca1L(1) - (lca1L(1) - lca2L(1)) * (-cpL(0) / 2 - cpR(0) / 2 + lca1L(0)) / (lca1L(0) - lca2L(0)),
			lca1L(2) - (lca1L(2) - lca2L(2)) * (-cpL(0) / 2 - cpR(0) / 2 + lca1L(0)) / (lca1L(0) - lca2L(0)) };

		Eigen::Vector3f uca12Lpr{
			cpL(0) / 2 + cpR(0) / 2,
			uca1L(1) - (uca1L(1) - uca2L(1)) * (-cpL(0) / 2 - cpR(0) / 2 + uca1L(0)) / (uca1L(0) - uca2L(0)),
			uca1L(2) - (uca1L(2) - uca2L(2)) * (-cpL(0) / 2 - cpR(0) / 2 + uca1L(0)) / (uca1L(0) - uca2L(0)) };

		Eigen::Vector3f lca12Rpr{
			cpL(0) / 2 + cpR(0) / 2,
			lca1R(1) - (lca1R(1) - lca2R(1)) * (-cpL(0) / 2 - cpR(0) / 2 + lca1R(0)) / (lca1R(0) - lca2R(0)),
			lca1R(2) - (lca1R(2) - lca2R(2)) * (-cpL(0) / 2 - cpR(0) / 2 + lca1R(0)) / (lca1R(0) - lca2R(0)) };

		Eigen::Vector3f uca12Rpr{
			cpL(0) / 2 + cpR(0) / 2,
			uca1R(1) - (uca1R(1) - uca2R(1)) * (-cpL(0) / 2 - cpR(0) / 2 + uca1R(0)) / (uca1R(0) - uca2R(0)),
			uca1R(2) - (uca1R(2) - uca2R(2)) * (-cpL(0) / 2 - cpR(0) / 2 + uca1R(0)) / (uca1R(0) - uca2R(0)) };

		float aLCAL = (lca3L(1) - lca12Lpr(1)) / (lca3L(2) - lca12Lpr(2));
		float bLCAL = -aLCAL * lca12Lpr(2) + lca12Lpr(1);

		float aUCAL = (uca3L(1) - uca12Lpr(1)) / (uca3L(2) - uca12Lpr(2));
		float bUCAL = -aUCAL * uca12Lpr(2) + uca12Lpr(1);

		float aLCAR = (lca3R(1) - lca12Rpr(1)) / (lca3R(2) - lca12Rpr(2));
		float bLCAR = -aLCAR * lca12Rpr(2) + lca12Rpr(1);

		float aUCAR = (uca3R(1) - uca12Rpr(1)) / (uca3R(2) - uca12Rpr(2));
		float bUCAR = -aUCAR * uca12Rpr(2) + uca12Rpr(1);


		float aICL;
		float aICR;
		float bICL;
		float bICR;

		// case if LEFT LCA and UCA are parallel
		if (abs(aLCAL - aUCAL) < slopePrecision)
		{
			aICL = aLCAL;
			bICL = cpL(1) - aLCAL * cpL(2);
		}
		// case if LEFT LCA and UCA are NOT parallel
		else
		{
			float ICLy = (aLCAL * bUCAL - aUCAL * bLCAL) / (aLCAL - aUCAL);
			float ICLz = (-bLCAL + bUCAL) / (aLCAL - aUCAL);


			aICL = (ICLy - cpL(1)) / (ICLz - cpL(2));
			bICL = -cpL(2) * (ICLy - cpL(1)) / (ICLz - cpL(2)) + cpL(1);
		}

		
		// case if RIGHT LCA and UCA are parallel
		if (abs(aLCAR - aUCAR) < slopePrecision)
		{
			aICR = aLCAR;
			bICR = cpR(1) - aLCAR * cpR(2);
		}
		// case if RIGHT LCA and UCA are NOT parallel
		else
		{
			float ICRy = (aLCAR * bUCAR - aUCAR * bLCAR) / (aLCAR - aUCAR);
			float ICRz = (-bLCAR + bUCAR) / (aLCAR - aUCAR);


			aICR = (ICRy - cpR(1)) / (ICRz - cpR(2));
			bICR = -cpR(2) * (ICRy - cpR(1)) / (ICRz - cpR(2)) + cpR(1);
		}

		float RCz = (bICL - bICR) / (aICR - aICL);
		float RCy = aICL * (bICL - bICR) / (aICR - aICL) + bICL;

		float rc_height =
			((cpR(1) - cpL(1)) * (cpL(2) - RCz) -
				(cpL(1) - RCy) * (cpR(2) - cpL(2))) /
			sqrt(pow((cpR(2) - cpL(2)), 2) + pow((cpR(1) - cpL(1)), 2));

		return rc_height;
	}

	float CalculateCasterTrail(Eigen::MatrixXf& lca3Mat, Eigen::MatrixXf& uca3Mat, Eigen::MatrixXf& spnMat, Eigen::MatrixXf& wcnMat, Eigen::MatrixXf& cpMat, int position)
	{
		int L = position;                  // L means left
		int R = cpMat.rows() - 1 - position;  // R means right

		Eigen::Vector3f cpL{ cpMat.row(L) };
		Eigen::Vector3f cpR{ cpMat.row(R) };
		Eigen::Vector3f wcn{ wcnMat.row(L) };
		Eigen::Vector3f spn{ spnMat.row(L) };
		Eigen::Vector3f lca3L{ lca3Mat.row(L) };
		Eigen::Vector3f uca3L{ uca3Mat.row(L) };



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
			return caster_trail;

		// negative caster trail
		else
			return -caster_trail;

	}
};


double optimisation_obj_res()
{
	Suspension susp{
		-2038.666, -411.709, -132.316,
		-2241.147, -408.195, -126.205,
		-2135, -600, -140,
		-2040.563, -416.249, -275.203,
		-2241.481, -417.314, -270.739,
		-2153, -578, -315,
		-2234.8, -411.45, -194.6,
		-2225, -582, -220,
		-2143.6, -620.5, -220.07,
		-2143.6, -595.5, -219.34,  
		210, 30, 30, 1,
		10, 0.01
	};

	int i = susp.CalculateMovement();

	return i;
}