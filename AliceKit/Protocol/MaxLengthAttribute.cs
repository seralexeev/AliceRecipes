using System;

namespace AliceKit.Protocol {
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class MaxLengthAttribute : Attribute {
    public int MaxLength { get; }

    public MaxLengthAttribute(int maxLength) {
      MaxLength = maxLength;
    }
  }
}
