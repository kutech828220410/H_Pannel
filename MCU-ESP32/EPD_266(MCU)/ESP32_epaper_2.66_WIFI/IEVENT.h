#ifndef IEVENT_H
#define IEVENT_H

template<class SenderType ,class ParaType> class EventPublisher ;

class NullType
{
};

// IEventHandler  是事件处理句柄，预定事件的类从此接口继承以实现事件处理函数
template<class SenderType ,class ParaType>
interface IEventHandler
{

public:
 virtual ~IEventHandler(){}
private:
 virtual void HandleEvent(SenderType sender ,ParaType para)  = 0 ;
 friend class EventPublisher<SenderType ,ParaType> ;
};

// IEvent 事件预定方通过此接口预定事件
template<class SenderType ,class ParaType>
interface IEvent
{
public:

 virtual ~IEvent(){}
 virtual void Register  (IEventHandler<SenderType ,ParaType>* handler) = 0 ;  
 virtual void UnRegister(IEventHandler<SenderType ,ParaType>* handler) = 0 ;
};


// IEventActivator 事件发布方通过此接口触发事件
template<class SenderType ,class ParaType>
interface IEventActivator
{
public:

 virtual ~IEventActivator(){}
 virtual void Invoke(SenderType sender ,ParaType para) = 0;
 virtual int  HandlerCount() = 0;
 virtual IEventHandler<SenderType ,ParaType>* GetHandler(int index) = 0;
};

// IEventPublisher 事件发布方发布事件相当于就是发布一个IEventPublisher派生类的对象
// 不过仅仅将该对象的IEvent接口发布即可。
template<class SenderType ,class ParaType>
interface IEventPublisher : public IEvent<SenderType ,ParaType> ,public IEventActivator<SenderType ,ParaType> 
{
};

// EventPublisher是IEventPublisher的默认实现
template<class SenderType ,class ParaType>
class EventPublisher :public IEventPublisher<SenderType ,ParaType>
{
private:
 SafeArrayList< IEventHandler<SenderType ,ParaType> > handerList ;
 IEventHandler<SenderType ,ParaType>* innerHandler ;

public:
 void Register(IEventHandler<SenderType ,ParaType>* handler) 
 {
  this->handerList.Add(handler) ;
 }

 void UnRegister(IEventHandler<SenderType ,ParaType>* handler)
 {
  this->handerList.Remove(handler) ;
 }

 void Invoke(SenderType sender ,ParaType para)
 {
  int count = this->handerList.Count() ;
  for(int i=0 ;i<count ;i++)
  {
   IEventHandler<SenderType ,ParaType>* handler = this->handerList.GetElement(i) ;
   handler->HandleEvent(sender ,para) ;
  }
 } 

 int  HandlerCount()
 {
  return this->handerList.Count() ;
 }

 IEventHandler<SenderType ,ParaType>* GetHandler(int index)
 {
  return this->handerList.GetElement(index) ;
 }
};

#endif
