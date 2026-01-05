using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai_Library.Enums
{
    public enum Env
    {
        SAI_API_KEY,
        SAI_API_BASE_URL
    }

    public static class EnvExtensions
    {
        public static string GetEnvValue(this Env env)
        {
            return Environment.GetEnvironmentVariable(env.ToString());
        }

        public static string GetEnvValue(string name)
        {
            try
            {
                if (Enum.TryParse<Env>(name, out var env))
                {
                    return env.GetEnvValue();
                }
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine($"Environment variable {name} not found: {e.Message}");
            }
            return null;
        }
    }
    
    public enum RequestMethod
    {
        GET,
        POST,
        PUT,
        PATCH,
        DELETE
    }
    

    /// <summary>
    /// Tipos de modelos disponíveis na API de IA
    /// </summary>
    public enum ModelType
    {
        /// <summary>
        /// Modelo de chat/conversa
        /// </summary>
        Chat = 0,
    
        /// <summary>
        /// Modelo de áudio (transcrição, síntese)
        /// </summary>
        Audio = 1,
    
        /// <summary>
        /// Modelo de imagem (geração, análise)
        /// </summary>
        Image = 2
    }
}