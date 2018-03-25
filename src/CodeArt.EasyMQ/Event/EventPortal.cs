﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.DTO;

namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 事件门户，可以发布/订阅/取消订阅事件
    /// </summary>
    public static class EventPortal
    {
        /// <summary>
        /// 当事件有订阅时发布事件
        /// </summary>
        /// <param name="name">事件的名称</param>
        /// <param name="arg">事件参数</param>
        public static void Publish(string eventName, DTObject arg)
        {
            var publisher = CreatePublisher();
            publisher.Publish(eventName, arg);
        }

        private static IPublisher CreatePublisher()
        {
            return GetPublisherFactory().Create();
        }

        /// <summary>
        /// 订阅远程事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="branch"></param>
        public static void Subscribe(string eventName, IEventHandler handler)
        {
            var subscriber = CreateSubscriber(eventName);
            subscriber.AddHandler(handler);
            subscriber.Accept();
        }

        private static ISubscriber CreateSubscriber(string eventName)
        {
            var config = EasyMQConfiguration.Current.EventConfig;
            var group = config.SubscriberGroup;
            return GetSubscriberFactory().Create(eventName, group);
        }


        public static void Cancel(string eventName)
        {
            var subscriber = CreateSubscriber(eventName);
            subscriber.Stop();
        }

        /// <summary>
        /// 主动释放事件资源
        /// </summary>
        /// <param name="eventName"></param>
        public static void Cleanup(string eventName)
        {
            var subscriber = CreateSubscriber(eventName);
            subscriber.Cleanup();
        }

        #region 获取和注册工厂

        internal static IPublisherFactory GetPublisherFactory()
        {
            return _publisherSetting.GetFactory();
        }

        private static FactorySetting<IPublisherFactory> _publisherSetting = new FactorySetting<IPublisherFactory>(() =>
        {
            var config = EasyMQConfiguration.Current.EventConfig;
            InterfaceImplementer imp = config.PublisherFactoryImplementer;
            if (imp != null)
            {
                return imp.GetInstance<IPublisherFactory>();
            }
            return null;
        });

        public static void Register(IPublisherFactory factory)
        {
            _publisherSetting.Register(factory);
        }



        internal static ISubscriberFactory GetSubscriberFactory()
        {
            return _subscriberSetting.GetFactory();
        }

        private static FactorySetting<ISubscriberFactory> _subscriberSetting = new FactorySetting<ISubscriberFactory>(() =>
        {
            var config = EasyMQConfiguration.Current.EventConfig;
            InterfaceImplementer imp = config.SubscriberFactoryImplementer;
            if (imp != null)
            {
                return imp.GetInstance<ISubscriberFactory>();
            }
            return null;
        });

        public static void Register(ISubscriberFactory factory)
        {
            _subscriberSetting.Register(factory);
        }



        #endregion

    }
}
