using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ClientAPP.VideoModule
{
    public class LayoutConfig
    {
        [XmlArray]
        public List<Layout> Layouts { get; set; }
    }

    /// <summary>
    /// videoControl排列
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 图片路径(用于显示在结构图)
        /// </summary>
        [XmlAttribute]
        public string ImgPath { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        [XmlAttribute]
        public int RowCount { get; set; }
        /// <summary>
        /// 列数
        /// </summary>
        [XmlAttribute]
        public int ColumnCount { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        [XmlAttribute]
        public bool Visible { get; set; }

        /// <summary>
        /// 屏幕数量
        /// </summary>
        [XmlAttribute]
        public int ScreenCount { get; set; }

        /// <summary>
        /// 视频控件列表
        /// </summary>
        public List<VideoControlLayout> ControlList { get; set; }


        public static Layout GetNormalLayou(string name, int rowCount, int columnCount, int screenCount=1)
        {
            Layout ret = new Layout() { ColumnCount = columnCount, RowCount = rowCount, Name = name, ScreenCount = screenCount, Visible = true, ControlList = new List<VideoControlLayout>() };
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    ret.ControlList.Add(new VideoControlLayout() { Column = j, ColumnSpan = 1, Row = i, RowSpan = 1, MaxColumn = 0, MaxColumnSpan = columnCount, MaxRow = 0, MaxRowSpan = rowCount });
                }
            }
            return ret;
        }



    }

    /// <summary>
    /// 单个control的坐标和大小
    /// </summary>
    public class VideoControlLayout
    {
        /// <summary>
        /// 视频控件
        /// </summary>
        [XmlIgnore]
        public VideoControl Control { get; set; }
        /// <summary>
        /// 起始行号
        /// </summary>
        [XmlAttribute]
        public int Row { get; set; }

        /// <summary>
        /// 行跨度
        /// </summary>
        [XmlAttribute]
        public int RowSpan { get; set; }

        /// <summary>
        /// 起始列号
        /// </summary>
        [XmlAttribute]
        public int Column { get; set; }

        /// <summary>
        /// 列跨度
        /// </summary>
        [XmlAttribute]
        public int ColumnSpan { get; set; }

        /// <summary>
        /// 最大化时起始行
        /// </summary>
        [XmlAttribute]
        public int MaxRow { get; set; }

        /// <summary>
        /// 最大化时行跨度
        /// </summary>
        [XmlAttribute]
        public int MaxRowSpan { get; set; }

        /// <summary>
        /// 最大化时起始列
        /// </summary>
        [XmlAttribute]
        public int MaxColumn { get; set; }

        /// <summary>
        /// 最大化时列跨度
        /// </summary>
        [XmlAttribute]
        public int MaxColumnSpan { get; set; }

        /// <summary>
        /// 总在最前
        /// </summary>
        [XmlAttribute]
        public bool IsTop { get; set; }

    }
}
