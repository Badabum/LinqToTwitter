﻿using System;
using System.Linq;
using System.Threading;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class StreamingDemo
    {
        public static void Run(TwitterContext twitterCtx)
        {
            //SamplesDemo(twitterCtx);
            FilterDemo(twitterCtx);
            //UserStreamDemo(twitterCtx);
            //UserStreamWithTimeoutDemo(twitterCtx);
            //SiteStreamDemo(twitterCtx);
        }

        private static void FilterDemo(TwitterContext twitterCtx)
        {
            twitterCtx.StreamingUserName = "";
            twitterCtx.StreamingPassword = "";

            if (twitterCtx.StreamingUserName == string.Empty ||
                twitterCtx.StreamingPassword == string.Empty)
            {
                Console.WriteLine("\n*** This won't work until you set the StreamingUserName and StreamingPassword on TwitterContext to valid values.\n");
                return;
            }

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.Streaming
                where strm.Type == StreamingType.Filter &&
                    strm.Track == "twitter"
                select strm)
            .StreamingCallback(strm =>
                {
                    Console.WriteLine(strm.Content + "\n");

                    if (count++ >= 2)
                    {
                        strm.CloseStream();
                    }
                })
            .SingleOrDefault();
        }

        private static void SamplesDemo(TwitterContext twitterCtx)
        {
            twitterCtx.StreamingUserName = "";
            twitterCtx.StreamingPassword = "";

            if (twitterCtx.StreamingUserName == string.Empty ||
                twitterCtx.StreamingPassword == string.Empty)
            {
                Console.WriteLine("\n*** This won't work until you set the StreamingUserName and StreamingPassword on TwitterContext to valid values.\n");
                return;
            }

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.Streaming
                where strm.Type == StreamingType.Sample
                select strm)
            .StreamingCallback(strm =>
            {
                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 10)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }

        private static void UserStreamDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            // the user stream is for whoever is authenticated
            // via the Authenticator passed to TwitterContext
            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.User
             select strm)
            .StreamingCallback(strm =>
            {
                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 25)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }


        private static void UserStreamWithTimeoutDemo(TwitterContext twitterCtx)
        {
            twitterCtx.ReadWriteTimeout = 3000;
            StreamContent strmCont = null;

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            // the user stream is for whoever is authenticated
            // via the Authenticator passed to TwitterContext
            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.User
             select strm)
            .StreamingCallback(strm =>
            {
                strmCont = strm;
                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 25)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();

            while (strmCont == null)
            {
                Console.WriteLine("Waiting on stream to initialize.");

                Thread.Sleep(10000);
            }

            Console.WriteLine("Stream is initialized. Now closing...");
            strmCont.CloseStream();
        }

        private static void SiteStreamDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.Site &&
                   //strm.With == "followings" &&
                   strm.Follow == "15411837"//,16761255"
             select strm)
            .StreamingCallback(strm =>
            {
                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 10)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }
    }
}
