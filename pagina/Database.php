<?php
class Database {
    private $host = 'localhost';       // Accedemos vía túnel SSH
    private $port = '9999';            // Puerto local del túnel
    private $user = 'armando';         // Usuario de la base de datos
    private $password = 'CONtrasena20.'; // Contraseña del usuario de BD
    private $dbname = 'iop';           // Nombre de la base de datos
    private $conn;

    // Constructor: conecta automáticamente
    public function __construct() {
        $this->connect();
    }

    // Conexión a PostgreSQL
    private function connect() {
        $connection_string = "host=$this->host port=$this->port dbname=$this->dbname user=$this->user password=$this->password";
        $this->conn = pg_connect($connection_string);

        if (!$this->conn) {
            die("Error de conexión: " . pg_last_error());
        }
    }

    // Ejecutar consulta SQL
    public function query($sql) {
        $result = pg_query($this->conn, $sql);

        if ($result === false) {
            return "Error en la consulta: " . pg_last_error($this->conn);
        }

        $data = [];
        while ($row = pg_fetch_assoc($result)) {
            $data[] = $row;
        }

        return $data;
    }

    // Cerrar la conexión
    public function close() {
        pg_close($this->conn);
    }
}
?>
