using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    enum HTTPStatus
    {
        OK = 200,
        NOT_FOUND = 404,
        INTERNAL_ERROR = 500,
        UNAUTHORIZED = 401
    }
}