namespace SqlNative;

public partial class MySqlFunctions
{
    /// <summary>
    /// Returns the X coordinate of the point <paramref name="point"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. Returns NULL if the argument is not a Point.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_X(ST_GeomFromText('POINT(1 2)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_X(QueryPart point)
    {
        return $"ST_X({point})";
    }

    /// <summary>
    /// Returns the Y coordinate of the point <paramref name="point"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. Returns NULL if the argument is not a Point.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_Y(ST_GeomFromText('POINT(1 2)')); -&gt; 2
    /// </code>
    /// </example>
    public static string ST_Y(string point)
    {
        return $"ST_Y({point})";
    }

    /// <summary>
    /// Returns the latitude of the point <paramref name="point"/> in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.12 and MariaDB 10.4. <paramref name="unit"/> is optional (default degree).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_LATITUDE(ST_GeomFromText('POINT(45.76 -80.12)'), 'degree'); -&gt; 45.76
    /// </code>
    /// </example>
    public static string ST_LATITUDE(string point, string unit)
    {
        return $"ST_LATITUDE({point}, {unit})";
    }

    /// <summary>
    /// Returns the longitude of the point <paramref name="point"/> in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 8.0.12 and MariaDB 10.4. <paramref name="unit"/> is optional (default degree).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_LONGITUDE(ST_GeomFromText('POINT(45.76 -80.12)'), 'degree'); -&gt; -80.12
    /// </code>
    /// </example>
    public static string ST_LONGITUDE(string point, string unit)
    {
        return $"ST_LONGITUDE({point}, {unit})";
    }

    /// <summary>
    /// Returns the distance between <paramref name="g1"/> and <paramref name="g2"/> in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. <paramref name="unit"/> is optional (default unit of SRID).
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_DISTANCE(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POINT(4 5)')); -&gt; 5
    /// </code>
    /// </example>
    public static string ST_DISTANCE(string g1, string g2, string unit)
    {
        return $"ST_DISTANCE({g1}, {g2}, {unit})";
    }

    /// <summary>
    /// Returns the distance between <paramref name="g1"/> and <paramref name="g2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_DISTANCE(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POINT(4 5)'), 'metre'); -&gt; 5
    /// </code>
    /// </example>
    public static string ST_DISTANCE(string g1, string g2)
    {
        return $"ST_DISTANCE({g1}, {g2})";
    }

    /// <summary>
    /// Returns the area of the polygon <paramref name="poly"/> in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. <paramref name="unit"/> is optional.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_AREA(ST_GeomFromText('POLYGON((0 0, 4 0, 4 4, 0 4, 0 0))')); -&gt; 16
    /// </code>
    /// </example>
    public static string ST_AREA(string poly, string unit)
    {
        return $"ST_AREA({poly}, {unit})";
    }

    /// <summary>
    /// Returns the area of the polygon <paramref name="poly"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_AREA(ST_GeomFromText('POLYGON((0 0, 4 0, 4 4, 0 4, 0 0))'), 'metre'); -&gt; 16
    /// </code>
    /// </example>
    public static string ST_AREA(string poly)
    {
        return $"ST_AREA({poly})";
    }

    /// <summary>
    /// Returns the length of the linestring <paramref name="linestring"/> in the specified <paramref name="unit"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. <paramref name="unit"/> is optional.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_LENGTH(ST_GeomFromText('LINESTRING(0 0, 3 0, 3 4, 0 4)')); -&gt; 11
    /// </code>
    /// </example>
    public static string ST_LENGTH(string linestring, string unit)
    {
        return $"ST_LENGTH({linestring}, {unit})";
    }

    /// <summary>
    /// Returns the length of the linestring <paramref name="linestring"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_LENGTH(ST_GeomFromText('LINESTRING(0 0, 3 0, 3 4, 0 4)'), 'metre'); -&gt; 11
    /// </code>
    /// </example>
    public static string ST_LENGTH(string linestring)
    {
        return $"ST_LENGTH({linestring})";
    }

    /// <summary>
    /// Returns the GeoJSON representation of the geometry <paramref name="geom"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. <paramref name="maxDecimalDigits"/> and <paramref name="options"/> are optional.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASGEOJSON(ST_GeomFromText('POINT(1 1)'), 4); -&gt; {"type": "Point", "coordinates": [1, 1]}
    /// </code>
    /// </example>
    public static string ST_ASGEOJSON(string geom, string maxDecimalDigits, string options)
    {
        return $"ST_ASGEOJSON({geom}, {maxDecimalDigits}, {options})";
    }

    /// <summary>
    /// Returns the GeoJSON representation of the geometry <paramref name="geom"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASGEOJSON(ST_GeomFromText('POINT(1 1)')); -&gt; {"type": "Point", "coordinates": [1.0, 1.0]}
    /// </code>
    /// </example>
    public static string ST_ASGEOJSON(string geom)
    {
        return $"ST_ASGEOJSON({geom})";
    }

    /// <summary>
    /// Returns the WKT representation of the geometry <paramref name="geom"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_GeomFromText('POINT(1 1)')); -&gt; 'POINT(1 1)'
    /// </code>
    /// </example>
    public static string ST_ASTEXT(string geom)
    {
        return $"ST_ASTEXT({geom})";
    }

    /// <summary>
    /// Returns 1 if geometry <paramref name="g1"/> contains <paramref name="g2"/>, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_CONTAINS(ST_GeomFromText('POLYGON((0 0, 4 0, 4 4, 0 4, 0 0))'), ST_GeomFromText('POINT(1 1)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_CONTAINS(string g1, string g2)
    {
        return $"ST_CONTAINS({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometry <paramref name="g1"/> is within <paramref name="g2"/>, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_WITHIN(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POLYGON((0 0, 4 0, 4 4, 0 4, 0 0))')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_WITHIN(string g1, string g2)
    {
        return $"ST_WITHIN({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometries <paramref name="g1"/> and <paramref name="g2"/> intersect, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_INTERSECTS(ST_GeomFromText('LINESTRING(0 0, 5 5)'), ST_GeomFromText('LINESTRING(2 0, 0 2)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_INTERSECTS(string g1, string g2)
    {
        return $"ST_INTERSECTS({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometries <paramref name="g1"/> and <paramref name="g2"/> are equal, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_EQUALS(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POINT(1 1)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_EQUALS(string g1, string g2)
    {
        return $"ST_EQUALS({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometries <paramref name="g1"/> and <paramref name="g2"/> are disjoint, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_DISJOINT(ST_GeomFromText('POINT(0 0)'), ST_GeomFromText('POINT(5 5)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_DISJOINT(string g1, string g2)
    {
        return $"ST_DISJOINT({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometries <paramref name="g1"/> and <paramref name="g2"/> cross, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_CROSSES(ST_GeomFromText('LINESTRING(0 0, 5 5)'), ST_GeomFromText('LINESTRING(2 0, 0 2)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_CROSSES(string g1, string g2)
    {
        return $"ST_CROSSES({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometries <paramref name="g1"/> and <paramref name="g2"/> overlap, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_OVERLAPS(ST_GeomFromText('POLYGON((0 0, 3 0, 3 3, 0 3, 0 0))'), ST_GeomFromText('POLYGON((2 2, 5 2, 5 5, 2 5, 2 2))')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_OVERLAPS(string g1, string g2)
    {
        return $"ST_OVERLAPS({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if geometry <paramref name="g1"/> touches <paramref name="g2"/>, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_TOUCHES(ST_GeomFromText('LINESTRING(0 0, 5 5)'), ST_GeomFromText('LINESTRING(5 5, 7 7)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_TOUCHES(string g1, string g2)
    {
        return $"ST_TOUCHES({g1}, {g2})";
    }

    /// <summary>
    /// Returns 1 if <paramref name="g"/> is geometrically valid, 0 otherwise.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ISVALID(ST_GeomFromText('LINESTRING(0 0, 5 5)')); -&gt; 1
    /// </code>
    /// </example>
    public static string ST_ISVALID(string g)
    {
        return $"ST_ISVALID({g})";
    }

    /// <summary>
    /// Returns a geometry constructed from the WKT representation <paramref name="wkt"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. Synonym for ST_GeomFromText.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_GEOMFROMTEXT('POINT(1 1)')); -&gt; 'POINT(1 1)'
    /// </code>
    /// </example>
    public static string ST_GEOMFROMTEXT(string wkt)
    {
        return $"ST_GEOMFROMTEXT({wkt})";
    }

    /// <summary>
    /// Returns a geometry constructed from the WKB representation <paramref name="wkb"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. Synonym for ST_GeomFromWKB.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_GEOMFROMWKB(ST_ASBINARY(ST_GeomFromText('POINT(1 1)')))); -&gt; 'POINT(1 1)'
    /// </code>
    /// </example>
    public static string ST_GEOMFROMWKB(string wkb)
    {
        return $"ST_GEOMFROMWKB({wkb})";
    }

    /// <summary>
    /// Returns the WKB representation of <paramref name="geom"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2. Synonym for ST_AsBinary.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT HEX(ST_ASBINARY(ST_GeomFromText('POINT(1 1)'))); -&gt; '0101000000000000000000F03F000000000000F03F'
    /// </code>
    /// </example>
    public static string ST_ASBINARY(string geom)
    {
        return $"ST_ASBINARY({geom})";
    }

    /// <summary>
    /// Returns the centroid of the geometry <paramref name="g"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_CENTROID(ST_GeomFromText('POLYGON((0 0, 4 0, 4 4, 0 4, 0 0))'))); -&gt; 'POINT(2 2)'
    /// </code>
    /// </example>
    public static string ST_CENTROID(string g)
    {
        return $"ST_CENTROID({g})";
    }

    /// <summary>
    /// Returns a geometry that is the union of <paramref name="g1"/> and <paramref name="g2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_UNION(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POINT(2 2)'))); -&gt; 'MULTIPOINT(1 1,2 2)'
    /// </code>
    /// </example>
    public static string ST_UNION(string g1, string g2)
    {
        return $"ST_UNION({g1}, {g2})";
    }

    /// <summary>
    /// Returns a geometry that is the difference of <paramref name="g1"/> and <paramref name="g2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_DIFFERENCE(ST_GeomFromText('POLYGON((0 0, 4 0, 4 4, 0 4, 0 0))'), ST_GeomFromText('POLYGON((2 2, 6 2, 6 6, 2 6, 2 2))'))); -&gt; 'POLYGON((0 0,4 0,4 2,2 2,2 4,0 4,0 0))'
    /// </code>
    /// </example>
    public static string ST_DIFFERENCE(string g1, string g2)
    {
        return $"ST_DIFFERENCE({g1}, {g2})";
    }

    /// <summary>
    /// Returns a geometry that is the point-set symmetric difference of <paramref name="g1"/> and <paramref name="g2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_SYMDIFFERENCE(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POINT(2 2)'))); -&gt; 'MULTIPOINT(1 1,2 2)'
    /// </code>
    /// </example>
    public static string ST_SYMDIFFERENCE(string g1, string g2)
    {
        return $"ST_SYMDIFFERENCE({g1}, {g2})";
    }

    /// <summary>
    /// Returns a geometry that is the intersection of <paramref name="g1"/> and <paramref name="g2"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_INTERSECTION(ST_GeomFromText('POINT(1 1)'), ST_GeomFromText('POINT(1 1)'))); -&gt; 'POINT(1 1)'
    /// </code>
    /// </example>
    public static string ST_INTERSECTION(string g1, string g2)
    {
        return $"ST_INTERSECTION({g1}, {g2})";
    }

    /// <summary>
    /// Returns a geometry that represents the envelope surrounding <paramref name="g"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_ENVELOPE(ST_GeomFromText('LINESTRING(0 0, 4 4)'))); -&gt; 'POLYGON((0 0,4 0,4 4,0 4,0 0))'
    /// </code>
    /// </example>
    public static string ST_ENVELOPE(string g)
    {
        return $"ST_ENVELOPE({g})";
    }

    /// <summary>
    /// Returns the GeoHash representation of the geometry <paramref name="g"/> with the given <paramref name="maxLength"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_GEOHASH(ST_GeomFromText('POINT(45.76 -80.12)'), 10); -&gt; 'dpzm7gsmfr'
    /// </code>
    /// </example>
    public static string ST_GEOHASH(string g, string maxLength)
    {
        return $"ST_GEOHASH({g}, {maxLength})";
    }

    /// <summary>
    /// Returns a Point value containing the longitude/latitude coordinates decoded from the <paramref name="geoHash"/> string.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7.5 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_ASTEXT(ST_POINTFROMGEOHASH('dpzm7gsmfr', 0)); -&gt; 'POINT(45.76 -80.12)'
    /// </code>
    /// </example>
    public static string ST_POINTFROMGEOHASH(string geoHash, string srid)
    {
        return $"ST_POINTFROMGEOHASH({geoHash}, {srid})";
    }

    /// <summary>
    /// Returns the SRID (Spatial Reference System ID) of the geometry <paramref name="g"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_SRID(ST_GeomFromText('POINT(1 1)', 4326)); -&gt; 4326
    /// </code>
    /// </example>
    public static string ST_SRID(string g)
    {
        return $"ST_SRID({g})";
    }

    /// <summary>
    /// Returns the type of the geometry <paramref name="g"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_GEOMETRYTYPE(ST_GeomFromText('POINT(1 1)')); -&gt; 'POINT'
    /// </code>
    /// </example>
    public static string ST_GEOMETRYTYPE(string g)
    {
        return $"ST_GEOMETRYTYPE({g})";
    }

    /// <summary>
    /// Returns the dimension of the geometry <paramref name="g"/>.
    /// </summary>
    /// <remarks>
    /// Available since MySQL 5.7 and MariaDB 10.2.
    /// </remarks>
    /// <example>
    /// <code>
    /// SELECT ST_DIMENSION(ST_GeomFromText('POINT(1 1)')) -&gt; 0
    /// </code>
    /// </example>
    public static string ST_DIMENSION(string g)
    {
        return $"ST_DIMENSION({g})";
    }
}