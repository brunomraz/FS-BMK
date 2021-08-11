#include "pch.h" // use stdafx.h in Visual Studio 2017 and earlier
#include <utility>
#include <limits.h>
#include "MathLibrary.h"

// DLL internal state variables:
static unsigned long long previous_;  // Previous value, if any
static unsigned long long current_;   // Current sequence value
static unsigned index_;               // Current seq. position

void fibonacci_init(
    const unsigned long long a,
    const unsigned long long b)
{
    index_ = 0;
    current_ = a;
    previous_ = b; // see special case when initialized
}

bool fibonacci_next()
{
    if ((ULLONG_MAX - previous_ < current_) ||
        (UINT_MAX == index_))
    {
        return false;
    }

    if (index_ > 0)
    {
        previous_ += current_;
    }
    std::swap(current_, previous_);
    ++index_;
    return true;
}

unsigned long long fibonacci_current()
{
    return current_;
}

unsigned fibonacci_index()
{
    return index_;
}