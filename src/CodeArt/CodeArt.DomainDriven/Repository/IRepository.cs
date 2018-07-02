using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface IRepository
    {
        /// <summary>
        /// ���ݱ�Ų��Ҷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IAggregateRoot Find(object id, QueryLevel level);
    }



    public interface IRepository<TRoot> : IRepository
        where TRoot : class, IAggregateRoot
    {
        /// <summary>
        /// ��������ӵ��ִ�
        /// </summary>
        /// <param name="obj"></param>
        void Add(TRoot obj);

        /// <summary>
        /// �޸Ķ����ڲִ��е���Ϣ
        /// </summary>
        /// <param name="obj"></param>
        void Update(TRoot obj);

        /// <summary>
        /// �Ӳִ���ɾ������
        /// </summary>
        /// <param name="obj"></param>
        void Delete(TRoot obj);

        /// <summary>
        /// ���ݱ�Ų��Ҷ���
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        new TRoot Find(object id, QueryLevel level);
    }
}