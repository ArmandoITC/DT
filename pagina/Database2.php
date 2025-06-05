<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

class Database2 {
    private $host = 'localhost';
    private $port = '5432';
    private $user = 'postgres';
    private $password = 'Armando18';
    private $dbname = 'Unity';
    private $conn;

    /* ---------- conexión ---------- */
    public function __construct() {
        $this->connect();
    }

    private function connect() {
        $str = "host={$this->host} port={$this->port} dbname={$this->dbname} user={$this->user} password={$this->password}";
        $this->conn = pg_connect($str);
        if (!$this->conn) {
            throw new \Exception('❌ Error de conexión: '.pg_last_error());
        }
    }

    /* ---------- NUEVO: exponer la conexión ---------- */
    public function getConn() {
        return $this->conn;
    }

    /* ---------- helpers ---------- */
    public function query($sql) {
        $res = pg_query($this->conn, $sql);
        if ($res === false) throw new \Exception('❌ Consulta: '.pg_last_error($this->conn));
        $rows=[];
        while($row=pg_fetch_assoc($res)) $rows[]=$row;
        return $rows;
    }

    public function execute($sql) {
        $res = pg_query($this->conn, $sql);
        if ($res === false) throw new \Exception('❌ Exec: '.pg_last_error($this->conn));
        return true;
    }

    public function close() { pg_close($this->conn); }
}
?>
