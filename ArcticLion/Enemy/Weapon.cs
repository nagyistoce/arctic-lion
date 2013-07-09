using System;

namespace ArcticLion
{
	public class Weapon : Node
	{
		public Projectile ProjectilePrototype;

		//TODO: Remove, for testing purposes
		public Weapon () : this(new EnemyBullet())
		{
		}

		public Weapon (Projectile projectilePrototype)
		{
			this.ProjectilePrototype = projectilePrototype;
		}
	}
}