using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using XemphimAPI.Controllers;

namespace XemphimAPI.Filter
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            int i = 0;
            if (SkipAuthorization(actionContext)) return;
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                if (usernamePasswordArray.Length<2)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var c_user = usernamePasswordArray[0];
                    var token = usernamePasswordArray[1];

                    // Replace this with your own system of security / means of validating credentials

                    MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
                    string sql = "";
                    sql = " SELECT max(id) id from t_token where  id_user=" + c_user;

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adap.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (String.Compare(ds.Tables[0].Rows[0]["id"].ToString(), token) == 0)
                        {
                            var principal = new GenericPrincipal(new GenericIdentity(c_user), null);
                            Thread.CurrentPrincipal = principal;

                            return;
                        }
                        else
                        {
                            i = 1;
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.SeeOther);
                        }
                    }
                }
                
            }

            HandleUnathorized(actionContext,i);
        }

        private static void HandleUnathorized(HttpActionContext actionContext,int i)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:4200");
            if (i == 1)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.SeeOther);
                actionContext.Response.Headers.Add("api_key", "1234");
            }
        }
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }

}