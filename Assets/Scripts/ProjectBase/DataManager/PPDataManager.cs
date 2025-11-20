using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class PPDataManager : SingletonBase<PPDataManager>
{
    /// <summary>
    /// 存储数据
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <param name="keyName">数据的唯一键</param>
    /// 

    int[] abc;
    public void SaveData(object data, string keyName)
    {
        Type dataType = data.GetType();
        FieldInfo info;
        //GetFields可以获取自定义类中的所有字段（BlindingFlags为筛选，这里获取了静态、非公有、公有的字段）
        FieldInfo[] infos = dataType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        string saveName;
        //设置一套命名规则 使每个数据的存储名是唯一的
        //传入名称 + 类名称 + 字段类名 + 字段名
        for (int i = 0; i < infos.Length; i++)
        {
            info = infos[i];
            saveName = keyName + "_" + dataType.Name + "_" + info.FieldType.Name + "_" + info.Name;
            SaveValue(info.GetValue(data), saveName);
        }
    }

    private void SaveValue(object value, string saveName)
    {
        Type fieldType = value.GetType();

        if (fieldType == typeof(int))
        {
            Debug.Log($"存储了int数据：{saveName}，值为{value}");
            PlayerPrefs.SetInt(saveName, (int)value);
        }
        else if (fieldType == typeof(float))
        {
            Debug.Log($"存储了float数据：{saveName}，值为{value}");
            PlayerPrefs.SetFloat(saveName, (float)value);
        }
        else if (fieldType == typeof(string))
        {
            Debug.Log($"存储了string数据：{saveName}，值为{value}");
            PlayerPrefs.SetString(saveName, value.ToString());
        }
        else if (fieldType == typeof(bool))
        {
            Debug.Log($"存储了bool数据：{saveName}，值为{((bool)value ? 1 : 0)}");
            PlayerPrefs.SetInt(saveName, (bool)value ? 1 : 0);
        }
        //通过判断传入参数是否是Ilist的子类来判断是否是list类型
        //（不好直接判断因为List会有泛型）
        else if (typeof(IList).IsAssignableFrom(fieldType))
        {
            IList lst = value as IList;
            Debug.Log($"存储了List数据：{saveName}，其长度为{lst.Count}");

            PlayerPrefs.SetInt(saveName, lst.Count);

            for (int i = 0; i < lst.Count; i++)
            {
                SaveValue(lst[i], $"{saveName}_{i}");
            }
        }
        //Dictionary同理 可传入IDictionary
        else if (typeof(IDictionary).IsAssignableFrom(fieldType))
        {
            IDictionary dic = value as IDictionary;
            Debug.Log($"存储了Dictionary数据：{saveName}，其长度为{dic.Count}");
            PlayerPrefs.SetInt(saveName, dic.Count);

            int index = 0;
            foreach (object item in dic.Keys)
            {
                //SaveValue(item, $"{saveName}_Key_{item}_{index}");
                //SaveValue(dic[item], $"{saveName}_Value_{dic[item]}_{index}");

                SaveValue(item, $"{saveName}_Key_{index}");
                SaveValue(dic[item], $"{saveName}_Value_{index}");

                index++;
            }
        }
        else
        {
            SaveData(value, saveName);
        }

        PlayerPrefs.Save();
    }




    public object LoadData(Type type, string keyName)
    {
        //实例化一个对象，然后向里面填充读取的数据，后返回
        object data = Activator.CreateInstance(type);

        FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        string loadKeyname;
        FieldInfo info;
        for (int i = 0; i < infos.Length; i++)
        {
            info = infos[i];
            //命名规则一定要与存储时的命名规则相同
            //传入名称 + 类名称 + 字段类名 + 字段名
            loadKeyname = keyName + "_" + type.Name + "_" + info.FieldType.Name + "_" + info.Name;

            //填充数据
            //.SetValue(字段所在类，赋予字段的值)
            info.SetValue(data, LoadValue(info.FieldType, loadKeyname));


        }
        return data;
    }


    private object LoadValue(Type fieldType, string loadName)
    {
        object field = new object();

        if (fieldType == typeof(int))
        {
            field = PlayerPrefs.GetInt(loadName);
            Debug.Log($"读取了int数据：{loadName}，值为{field}");
        }
        else if (fieldType == typeof(float))
        {
            field = PlayerPrefs.GetFloat(loadName);
            Debug.Log($"读取了float数据：{loadName}，值为{field}");
        }
        else if (fieldType == typeof(string))
        {
            field = PlayerPrefs.GetString(loadName);
            Debug.Log($"读取了string数据：{loadName}，值为{field}");
        }
        else if (fieldType == typeof(bool))
        {
            field = PlayerPrefs.GetInt(loadName) == 1 ? true : false;
            Debug.Log($"读取了bool数据：{loadName}，值为{field}");
        }
        else if (typeof(IList).IsAssignableFrom(fieldType))
        {
            IList list;
            int count = PlayerPrefs.GetInt(loadName);
            Debug.Log($"读取了List长度的数据：{loadName}，值为{count}");

            //数组和list都继承IList，需要进行判断
            //若是数组，数组没有无参构造函数，需要先传入长度(此处只区分了一维数组，无法构建二维数组)

            if (typeof(Array).IsAssignableFrom(fieldType))
                list = Activator.CreateInstance(fieldType, count) as IList;
            else
                list = Activator.CreateInstance(fieldType) as IList;

            object value;

            for (int i = 0; i < count; i++)
            {

                //判断是否为数组，如果是则 是获取 元素类型
                //若不是 则为list 则需要获取泛型类型
                if (typeof(Array).IsAssignableFrom(fieldType))
                {
                    value = LoadValue(fieldType.GetElementType(), loadName + "_" + i);
                    list[i] = value;
                }
                else
                {
                    value = LoadValue(fieldType.GetGenericArguments()[0], loadName + "_" + i);
                    list.Add(value);
                }


            }
            field = list;
        }
        else if (typeof(IDictionary).IsAssignableFrom(fieldType))
        {
            IDictionary Idic = Activator.CreateInstance(fieldType) as IDictionary;
            int count = PlayerPrefs.GetInt(loadName);
            Debug.Log($"读取了Dictionary长度的数据：{loadName}，值为{count}");


            object value;
            object key;
            for (int i = 0; i < count; i++)
            {
                key = LoadValue(fieldType.GetGenericArguments()[0], loadName + "_Key_" + i);
                value = LoadValue(fieldType.GetGenericArguments()[1], loadName + "_Value_" + i);
                Idic.Add(key, value);
            }

            field = Idic;

        }
        else
        {
            field = LoadData(fieldType, loadName);
        }

        return field;
    }





}
