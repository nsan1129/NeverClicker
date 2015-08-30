﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NeverClicker {
	[Serializable]
	public class GameTask : IComparable<GameTask>, ISerializable {
		public DateTime MatureTime { get; private set; }
		public uint CharacterIdx { get; private set; }
		public TaskKind Kind { get; private set; }

		public GameTask(DateTime matureTime, uint characterIdx, TaskKind kind) {
			this.MatureTime = matureTime;
			this.CharacterIdx = characterIdx;
			this.Kind = kind;
		}

		public int CompareTo(GameTask task) {
			return this.MatureTime.Ticks.CompareTo(task.MatureTime);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("MatureTime", MatureTime);
			info.AddValue("CharacterIdx", CharacterIdx);
			info.AddValue("TaskKind", Kind);
		}

		public void AddTicks(int ticks) {
			this.MatureTime = this.MatureTime.AddTicks(ticks);
        }
	}

	public enum TaskKind {
		Invocation,
		Profession
	}
}
