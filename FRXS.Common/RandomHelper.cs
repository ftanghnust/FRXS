using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRXS.Common
{
    public static class RandomHelper
    {

        /// <summary>
        /// 随机码类型
        /// </summary>
        public enum RandomCodeType
        {
            /// <summary>
            /// 数字
            /// </summary>
            Number = 1,
            /// <summary>
            /// 字母
            /// </summary>
            Letters = 2,
            /// <summary>
            /// 数字和字母
            /// </summary>
            NumberAndLetters = 3,
            /// <summary>
            /// 数字、字母和字符
            /// </summary>
            All = 4
        }

        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="codeCount">长度</param>
        /// <param name="codeType">随机码类型</param>
        /// <returns></returns>
        public static string CreateRandomCode(int codeCount, RandomCodeType codeType = RandomCodeType.Number)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9";
            switch (codeType)
            {
                case RandomCodeType.Number:
                    break;
                case RandomCodeType.Letters:
                    allChar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,w,x,y,z";
                    break;
                case RandomCodeType.NumberAndLetters:
                    allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,w,x,y,z";
                    break;
                case RandomCodeType.All:
                    allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,w,x,y,z,!,@,#,$,%,^,&,*,(,),{,},[,],:";
                    break;
                default:
                    break;
            }

            //里面的字符你可以自己加啦
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random random = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    random = new Random(i * temp * (int)DateTime.Now.Ticks);
                }
                int t = random.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount, codeType);
                }
                temp = t;
                randomCode += allCharArray[temp];
            }
            return randomCode;
        }

    }
}
