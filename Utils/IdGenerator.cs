namespace PhysicsEngineCore.Utils{
    class IdGenerator{
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random random = new Random();

        public static string createId(int length){
            char[] id = new char[length];
            for(int i = 0;i < length;i++){
                id[i] = chars[random.Next(chars.Length)];
            }

            return new string(id);
        }

    }
}
