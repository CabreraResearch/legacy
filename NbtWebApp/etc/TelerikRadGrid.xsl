<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="lib.xsl" />

<xsl:output method="xml" omit-xml-declaration="yes" cdata-section-elements="NodeName Action"/>

<xsl:variable name="level">1</xsl:variable>

<xsl:variable name="newline">
<xsl:text>
</xsl:text>
</xsl:variable>


<xsl:template match="NbtTree">
  <xsl:element name="Nodes">
    <xsl:apply-templates>
      <xsl:with-param name="isroot">true</xsl:with-param>
      <xsl:with-param name="isfirstlevel">false</xsl:with-param>
    </xsl:apply-templates>
  </xsl:element>
</xsl:template>

<xsl:template match="NbtNodeGroup">
  <xsl:param name="isroot" />
  <xsl:param name="isfirstlevel" />
  <xsl:apply-templates>
    <xsl:with-param name="isroot" select="$isroot" />
    <xsl:with-param name="isfirstlevel" select="$isfirstlevel" />
  </xsl:apply-templates>
</xsl:template>

<xsl:template match="NbtNode">
  <xsl:param name="isroot" />
  <xsl:param name="isfirstlevel" />

  <xsl:choose>
    <xsl:when test="$isfirstlevel='true'">
      <xsl:element name="Node">
        <xsl:element name="NodeName">
          <xsl:variable name="thisiconfilename">
            <xsl:value-of select="@iconfilename" />
          </xsl:variable>
          <xsl:variable name="thisnodename">
            <xsl:value-of select="@nodename" />
          </xsl:variable>
          <![CDATA[<img src="]]><xsl:value-of select="$thisiconfilename"/><![CDATA["/>]]><xsl:value-of select="$thisnodename" />
        </xsl:element>
        <xsl:text>
        </xsl:text>
        <xsl:element name="NodeKey">
          <xsl:value-of select="@key" />
        </xsl:element>
        <xsl:text>
        </xsl:text>
        <xsl:element name="Action">
          <![CDATA[<table border="0" cellpadding="0" cellspacing="0" style="border: 0px solid black; margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;">
                     <tr>
                       <td style="border: 0px solid black; margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;">
                         <div title="Edit" class="divbutton" style="background: url('Images/buttons/buttons18.gif') 0px -54px no-repeat;" 
                              onmouseover="this.style.backgroundPosition = '-18px -54px';"
                              onmouseout="this.style.backgroundPosition = '0px -54px';"
                              onclick="openEditNodePopup(']]><xsl:call-template name="makejavascriptsafe">
                                                               <xsl:with-param name="string" select="@key"/>
                                                             </xsl:call-template><![CDATA[');" />&nbsp;
                      </td>
                      <td style="border: 0px solid black; margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px;">
                        <div title="Delete" class="divbutton" style="background: url('Images/buttons/buttons18.gif') 0px -72px no-repeat;" 
                             onmouseover="this.style.backgroundPosition = '-18px -72px';"
                             onmouseout="this.style.backgroundPosition = '0px -72px';"
                             onclick="openDeleteNodePopup(']]><xsl:call-template name="makejavascriptsafe">
                                                                <xsl:with-param name="string" select="@key"/>
                                                              </xsl:call-template><![CDATA[');" />
                      </td>
                     </tr>
                   </table>]]>
        </xsl:element>
        <xsl:text>
        </xsl:text>

        <xsl:apply-templates>
          <xsl:with-param name="isroot">false</xsl:with-param>
          <xsl:with-param name="isfirstlevel">false</xsl:with-param>
        </xsl:apply-templates>
      </xsl:element>

    </xsl:when>

    <xsl:otherwise>
      <xsl:apply-templates>
        <xsl:with-param name="isroot">false</xsl:with-param>
        <xsl:with-param name="isfirstlevel" select="$isroot" />
      </xsl:apply-templates>
    </xsl:otherwise>
  </xsl:choose>

</xsl:template>


<xsl:template match="NbtNodeProp">
    <xsl:variable name="thisname">
      <xsl:text>Prop_</xsl:text>
<!--
      <xsl:value-of select="@nodetypepropid" />
      <xsl:text>_</xsl:text>
-->
      <xsl:call-template name="makesafe">
          <xsl:with-param name="string" select="@name" />
      </xsl:call-template>
    </xsl:variable>

    <xsl:variable name="thisgestalt">
      <xsl:value-of select="@gestalt" />
    </xsl:variable>

    <xsl:element name="{$thisname}">
      <xsl:value-of select="$thisgestalt" />
    </xsl:element>
    <xsl:text>
    </xsl:text>
</xsl:template>

<xsl:template match="/">
  <xsl:apply-templates/>
</xsl:template>


</xsl:stylesheet>