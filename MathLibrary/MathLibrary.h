#pragma once

#ifdef MATHLIBRARY_EXPORTS
#define MATHLIBRARY_API __declspec(dllexport)
#else
#define MATHLIBRARY_API __declspec(dllimport)
#endif // MATHLIBRARY_EXPORTS

extern "C" MATHLIBRARY_API void fibonacci_init(const unsigned long long a, const unsigned long long b);

extern "C" MATHLIBRARY_API bool fibonacci_next();

extern "C" MATHLIBRARY_API unsigned long long fibonacci_current();

extern "C" MATHLIBRARY_API unsigned fibonacci_index();