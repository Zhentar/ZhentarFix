using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace ZhentarFix
{
	[AttributeUsage(AttributeTargets.Method)]
	public class DetourClassMethod : Attribute
	{

		public readonly Type fromClass;
		public readonly string fromMethod;

		public DetourClassMethod(Type fromClass, string fromMethod = null)
		{
			this.fromClass = fromClass;
			this.fromMethod = fromMethod;
		}

	}

	[AttributeUsage(AttributeTargets.Property)]
	public class DetourClassProperty : Attribute
	{

		public readonly Type fromClass;
		public readonly string fromProperty;

		public DetourClassProperty(Type fromClass, string fromProperty = null)
		{
			this.fromClass = fromClass;
			this.fromProperty = fromProperty;
		}

	}
	[StaticConstructorOnStartup]
	public static class Detours
	{
		public const BindingFlags UniversalBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		static Detours()
		{
			LongEventHandler.QueueLongEvent(Inject, "Initializing", true, null);
		}

		private static void Inject()
		{
			var toTypes = typeof(Detours).Assembly.GetTypes().Where(toType => toType.GetMethods(UniversalBindingFlags).Any(toMethod => toMethod.HasAttribute<DetourClassMethod>()) ||
																			  toType.GetProperties(UniversalBindingFlags).Any(toProperty => toProperty.HasAttribute<DetourClassProperty>()));

			foreach (var type in toTypes)
			{
				DetourMethods(type);
				DetourProperties(type);
			}
		}

		private static void DetourMethods(Type toType)
		{
			var toMethods = toType
				.GetMethods(UniversalBindingFlags)
				.Where(toMethod => toMethod.HasAttribute<DetourClassMethod>());
			
			foreach (var toMethod in toMethods)
			{
				DetourClassMethod attribute = null;
				if (toMethod.TryGetAttribute(out attribute))
				{
					var fromMethodName = attribute.fromMethod ?? toMethod.Name;
					MethodInfo fromMethod;
					try
					{   // Try to get method direct
						fromMethod = attribute.fromClass.GetMethod(fromMethodName, UniversalBindingFlags);
					}
					catch
					{   // May be ambiguous, try from parameter count
						fromMethod = attribute.fromClass.GetMethods(UniversalBindingFlags)
												  .FirstOrDefault(checkMethod =>
																 (
																	 (checkMethod.Name == fromMethodName) &&
																	 (checkMethod.ReturnType == toMethod.ReturnType) &&
																	 (checkMethod.GetParameters().Length == toMethod.GetParameters().Length)
																	));
					}
					if (!TryDetourFromTo(fromMethod, toMethod))
					{
						Log.Error($"Failed to Detour '{toType.FullName}.{toMethod.Name}'");
					}
				}
			}
		}

		private static void DetourProperties(Type toType)
		{
			var toProperties = toType.GetProperties(UniversalBindingFlags).Where(toProperty => toProperty.HasAttribute<DetourClassProperty>());

			foreach (var toProperty in toProperties)
			{
				DetourClassProperty attribute = null;
				if (toProperty.TryGetAttribute(out attribute))
				{
					var fromProperty = attribute.fromClass.GetProperty(attribute.fromProperty ?? toProperty.Name, UniversalBindingFlags);
					var toMethod = toProperty.GetGetMethod(true);
					if (toMethod != null)
					{   // Check for get method detour
						var fromMethod = fromProperty.GetGetMethod(true);
						TryDetourFromTo(fromMethod, toMethod);
					}
					toMethod = toProperty.GetSetMethod(true);
					if (toMethod != null)
					{   // Check for set method detour
						var fromMethod = fromProperty.GetSetMethod(true);
						TryDetourFromTo(fromMethod, toMethod);
					}
				}
			}
		}

		/**
            This is a basic first implementation of the IL method 'hooks' (detours) made possible by RawCode's work;
            https://ludeon.com/forums/index.php?topic=17143.0

            Performs detours, spits out basic logs and warns if a method is detoured multiple times.
        **/
		public static unsafe bool TryDetourFromTo(MethodInfo source, MethodInfo destination)
		{

			if (source == null)
			{
				Log.Error("Source MethodInfo is null: Detours");
				return false;
			}
			if (destination == null)
			{
				Log.Error("Destination MethodInfo is null: Detours");
				return false;
			}

			// keep track of detours and spit out some messaging
			string sourceString = source.DeclaringType.FullName + "." + source.Name + " @ 0x" + source.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2).ToString());
			string destinationString = destination.DeclaringType.FullName + "." + destination.Name + " @ 0x" + destination.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2).ToString());

			if (IntPtr.Size == sizeof(Int64))
			{
				// 64-bit systems use 64-bit absolute address and jumps
				// 12 byte destructive

				// Get function pointers
				long Source_Base = source.MethodHandle.GetFunctionPointer().ToInt64();
				long Destination_Base = destination.MethodHandle.GetFunctionPointer().ToInt64();

				// Native source address
				byte* Pointer_Raw_Source = (byte*)Source_Base;

				// Pointer to insert jump address into native code
				long* Pointer_Raw_Address = (long*)(Pointer_Raw_Source + 0x02);

				// Insert 64-bit absolute jump into native code (address in rax)
				// mov rax, immediate64
				// jmp [rax]
				*(Pointer_Raw_Source + 0x00) = 0x48;
				*(Pointer_Raw_Source + 0x01) = 0xB8;
				*Pointer_Raw_Address = Destination_Base; // ( Pointer_Raw_Source + 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 )
				*(Pointer_Raw_Source + 0x0A) = 0xFF;
				*(Pointer_Raw_Source + 0x0B) = 0xE0;

			}
			else
			{
				// 32-bit systems use 32-bit relative offset and jump
				// 5 byte destructive

				// Get function pointers
				int Source_Base = source.MethodHandle.GetFunctionPointer().ToInt32();
				int Destination_Base = destination.MethodHandle.GetFunctionPointer().ToInt32();

				// Native source address
				byte* Pointer_Raw_Source = (byte*)Source_Base;

				// Pointer to insert jump address into native code
				int* Pointer_Raw_Address = (int*)(Pointer_Raw_Source + 1);

				// Jump offset (less instruction size)
				int offset = (Destination_Base - Source_Base) - 5;

				// Insert 32-bit relative jump into native code
				*Pointer_Raw_Source = 0xE9;
				*Pointer_Raw_Address = offset;
			}

			// done!
			return true;
		}

	}
}
